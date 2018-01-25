using System;
using System.Security.Claims;
using System.Threading.Tasks;
using CashLoanTool.Logic;
using CashLoanTool.EntityModels;
using CashLoanTool.Filters;
using CashLoanTool.Indus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog;
using CashLoanTool.ViewModels;
using System.Linq;
using CashLoanTool.Const;
using System.Collections.Generic;

namespace CashLoanTool.Helper
{
    [Route("API/RequestListing/[action]")]
    [Authorize]
    [CustomExceptionFilterAttribute] //use to catch unhandle Action Ex
    public class RequestListingController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private CLToolContext _context;
        private IConfiguration _config;
        private ICustomerAdapter _indus;
        //private IMemoryCache _cache;
        public RequestListingController(CLToolContext context, IConfiguration config, ICustomerAdapter indusAdapter)
        {
            _context = context;
            _config = config;
            _indus = indusAdapter;
            //_cache = memoryCache;
        }
        [HttpPost]
        public async Task<IActionResult> CheckContract([FromBody] PostWrapper post)
        {
            //Trim & clean special chars here
            var contractId = post.Post?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(contractId))
                return BadRequest();
            using (_context)
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                //Check if any request with this contract id exists
                var rq = await _context.Request.SingleOrDefaultAsync(r => r.LoanNo == contractId);
                if(rq != null)
                {
                    //Not valid
                    return Ok(new ResultWrapper() { Message = $"Khách hàng này đã request với ID: {rq.RequestId}" , Valid = false });
                }
                //Get info from indus
                var customerInfo = await _indus.GetCustomerInfo(contractId.Trim());
                //Check if customer meet bussiness' requirement
                if (CustomerValidator.CheckAndClean(customerInfo, contractId, out var mess, out var cleaned))    
                {
                    //Valid
                    return Ok(new ResultWrapper() { Message = mess, Valid = true });
                }
                else
                {
                    //Not valid
                    //Return Valid = false
                    return Ok(new ResultWrapper() { Message = mess, Valid = false });
                }
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] CreateRequestPost content)
        {
            //Trim & clean special chars here
            var contractId = content.ContractId?.Trim() ?? string.Empty;
            int issuerId = content.IssuePlace;
            int pobId = content.Pob;
            if (string.IsNullOrEmpty(contractId))
                return BadRequest();
            if (!CityList.CityTranslater(issuerId, pobId, out string issuer, out string pob))
                return BadRequest();
            var currentUser = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            using (_context)
            {
                //Get customer info from indus & strip vietnamese accents
                var customerInfo = await _indus.GetCustomerInfo(contractId);
                //double check incase client got modified intentionally
                if (CustomerValidator.CheckAndClean(customerInfo, contractId, out var mess, out var cleaned))
                {
                    if (cleaned == null) throw new InvalidOperationException();
                    //update input infomation
                    cleaned.Pob = pob;
                    cleaned.Issuer = issuer;
                    var request = new Request()
                    {
                        LoanNo = contractId,
                        RequestCreateTime = DateTime.Now,
                        RequestType = "OpenAccount", //Hardcoded as HDB request
                        Username = currentUser,
                        //Signature = "xxx" //Hardcoded as HDB request
                    };
                    request.CustomerInfo.Add(cleaned);
                    _context.Request.Add(request);
                    await _context.SaveChangesAsync();
                    //Request acccepted
                    return Ok(new ResultWrapper() { Message = $"Request thành công! ID: {request.RequestId}", Valid = true });
                }
                else
                {
                    //Check failed
                    //This should not ever happen unless client got tempered with
                    logger.Error($"CreateRequest CustomerValidator.Check Failed contractId: {contractId}");
                    return Ok(new ResultWrapper() { Message = mess, Valid = false });
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> FetchModel([FromQuery] int page = 1, [FromQuery] string by = "RequestId", [FromQuery] bool asc = false)
        {
            using (_context)
            {
               _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                return Ok(await CreateModel(_context, HttpContext, page, by, asc));
            }
        }
        //Move ability related logic to somewhere else?
        internal static async Task<RequestListingViewModel> CreateModel(CLToolContext context, HttpContext httpContext, int pageNum, string orderBy, bool asc)
        {
            if (httpContext.User.HasClaim(c => c.Value == AbilityNames.SeeAllRequests))
            {
                return await ModelFactory.CreateRequestListingModel(RequestsQuery.AllRequests(context), pageNum, orderBy, asc);
            }
            return await ModelFactory.CreateRequestListingModel(RequestsQuery.RequestsByDivision(context, SessionStore.ForceGetDevision(httpContext, context)), pageNum, orderBy, asc);
        }
    }
}