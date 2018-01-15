﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CashLoanTool.BussinessRules;
using CashLoanTool.EntityModels;
using CashLoanTool.Filters;
using CashLoanTool.Helper;
using CashLoanTool.Indus;
using CashLoanTool.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog;

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
                //Check if any request with this contract id exists
                var rq = await _context.Request.SingleOrDefaultAsync(r => string.Compare(r.LoanNo, contractId, true) == 0);
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
#if DEBUG
                return Ok(await GetModel(_context, SessionStore.ForceGetDevision(this.HttpContext, _context), page, by, asc));
#else
                return Ok(await GetModel(_context, SessionStore.ForceGetDevision(this.HttpContext, _context), page, by, asc));
                //return Ok(await GetModel(_context, SessionStore.GetDevision(this.HttpContext), page, by, asc));
#endif

            }
        }

        private static IOrderedQueryable<Request> OrderTranslater(IQueryable<Request> query, string orderBy, bool asc)
        {
            switch (orderBy)
            {
                case "RequestCreateTime":
                    if(!asc)
                        return query.OrderByDescending(r => r.RequestCreateTime);
                    return query.OrderBy(r => r.RequestCreateTime);
                case "RequestId":
                    if (!asc)
                        return query.OrderByDescending(r => r.RequestId);
                    return query.OrderBy(r => r.RequestId);
                //Others are not NYI
                default:
                    return query.OrderBy(r => r.RequestId);
            }
        }
        internal static async Task<RequestListingViewModel> GetModel(CLToolContext context, string division, int pageNum, string orderBy, bool asc)
        {
            int getPage = pageNum < 1 ? 1 : pageNum;
            int excludedRows = (getPage - 1) * RequestListingViewModel.ItemPerPage;
            //User can only see rq from same Division
            var query = context.Request.Where(r => r.UsernameNavigation.DivisionName == division);
            var totalRows = await query.CountAsync();
            var ordered = OrderTranslater(query, orderBy, asc);
            var model = new RequestListingViewModel
            {
                Requests = await ordered
                                .Skip(excludedRows)
                                .Take(RequestListingViewModel.ItemPerPage)
                                .Include(r => r.CustomerInfo)
                                .Include(r => r.Response)
                                .ToListAsync(),
                OnPage = pageNum,
                OrderAsc = asc,
                OrderBy = orderBy,
                Division = division
            };
            model.UpdatePagination(totalRows);
            return model;
        }
        internal static async Task<RequestListingViewModel> GetModelByUser(CLToolContext context, string userName, int pageNum, string orderBy, bool asc)
        {
            int getPage = pageNum < 1 ? 1 : pageNum;
            int excludedRows = (getPage - 1) * RequestListingViewModel.ItemPerPage;
            //var user = context.User.SingleOrDefaultAsync(u => u.Username == userName);
            //User can only see own requests
            var query = context.Request.Where(r => r.Username == userName);
            var totalRows = await query.CountAsync();
            var ordered = OrderTranslater(query, orderBy, asc);
            var model = new RequestListingViewModel
            {
                Requests = await ordered
                                .Skip(excludedRows)
                                .Take(RequestListingViewModel.ItemPerPage)
                                .Include(r => r.CustomerInfo)
                                .Include(r => r.Response)
                                .ToListAsync(),
                OnPage = pageNum,
                OrderAsc = asc,
                OrderBy = orderBy,
            };
            model.UpdatePagination(totalRows);
            return model;
        }
    }
}