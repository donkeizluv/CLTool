using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using CashLoanTool.BussinessRules;
using CashLoanTool.EntityModels;
using CashLoanTool.Helper;
using CashLoanTool.Indus;
using CashLoanTool.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using NLog;

namespace CashLoanTool.Controllers
{
    public class PostWrapper
    {
        public string Post { get; set; }
    }
    public class CustomerCheck
    {
        public bool Valid { get; set; }
        public string Message { get; set; }
    }

    [Route("API/RequestListing/[action]")]
    [Authorize]
    public class RequestListingController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private CLToolContext _context;
        private IConfiguration _config;
        private IIndusAdapter _indus;
        //private IMemoryCache _cache;
        public RequestListingController(CLToolContext context, IConfiguration config, IIndusAdapter indusAdapter)
        {
            _context = context;
            _config = config;
            _indus = indusAdapter;
            //_cache = memoryCache;
        }
        [HttpPost]
        public IActionResult CheckContract([FromBody] PostWrapper content)
        {
            //Trim & clean special chars here
            var contractId = content.Post.Trim();
            if (string.IsNullOrEmpty(contractId))
                return BadRequest();
            using (_context)
            {
                //Check if any request with this contract id exists
                var rq = _context.Request.Where(r => string.Compare(r.LoanNo, contractId, true) == 0);
                if(rq.Count() > 0)
                {
                    //Not valid
                    var rqId = rq.First().RequestId;
                    return Ok(new CustomerCheck() { Message = $"Khách hàng này đã request với ID: {rqId}" , Valid = false });
                }
                //Get info from indus
                var customerInfo = _indus.GetCustomerInfoIndus(contractId.Trim());
                //Check if customer meet bussiness' requirement
                if (CustomerValidator.Check(customerInfo, out var mess))    
                {
                    //Valid
                    return Ok(new CustomerCheck() { Message = $"Khách hàng hợp lệ. Tên: {customerInfo.FullName}, CMND: {customerInfo.IdentityCard}", Valid = true });
                }
                else
                {
                    //Not valid
                    return Ok(new CustomerCheck() { Message = mess, Valid = false });
                }

            }
        }
        [HttpPost]
        public IActionResult CreateRequest([FromBody] PostWrapper content)
        {
            //Trim & clean special chars here
            var contractId = content.Post.Trim();
            if (string.IsNullOrEmpty(contractId))
                return BadRequest();
            var currentUser = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            using (_context)
            {
                //Get customer info from indus & strip vietnamese accents
                var customerInfo = _indus.GetCustomerInfoIndus(contractId);
                try
                {
                    //double check incase client got modified intentionally
                    if (CustomerValidator.Check(customerInfo, out var mess))
                    {
                        var request = new Request()
                        {
                            LoanNo = contractId,
                            RequestCreateTime = DateTime.Now,
                            RequestType = "OpenAccount", //Hardcoded as HDB request
                            Username = currentUser,
                            Signature = "xxx" //Hardcoded as HDB request
                        };
                        request.CustomerInfo.Add(customerInfo);
                        _context.Request.Add(request);
                        _context.SaveChanges();
                        //Request acccepted
                        return Ok(new CustomerCheck() { Message = $"Request thành công! ID: {request.RequestId}", Valid = true });
                    }
                    else
                    {
                        //Check failed
                        return Ok(new CustomerCheck() { Message = mess, Valid = false });
                    }
                }
                catch (DbUpdateException ex)
                {
                    EnviromentHelper.LogException(ex, logger);
                    throw;
                }
            }
        }

        [HttpGet]
        public IActionResult FetchModel([FromQuery] int page = 1, [FromQuery] string by = "", [FromQuery] bool asc = true)
        {
            using (_context)
            {
                by = string.IsNullOrEmpty(by) ? "AcctNo" : by;
                var currentUser = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                return Ok(GetModel(_context, currentUser, page, by, asc));
            }
        }
        private static Func<Request, object> OrderTranslater(string orderBy)
        {
            switch (orderBy)
            {
                case "AcctNo":
                    return i => i.HasResponse;
                //Others are not NYI
                default:
                    return i => i.HasResponse;
            }
        }
        internal static RequestListingModel GetModel(CLToolContext context, string userName, int pageNum, string orderBy, bool asc)
        {
            var model = new RequestListingModel
            {
                Requests = GetRequest(context, userName, out var totalRows, pageNum, orderBy, asc),
                OnPage = pageNum,
                OrderAsc = asc,
                OrderBy = orderBy,
            };
            model.UpdatePagination(totalRows);
            return model;
        }
        internal static List<Request> GetRequest(CLToolContext context, string userName, out int totalRows, int pageNum, string orderBy, bool asc)
        {
            int getPage = pageNum < 1 ? 1 : pageNum;
            int excludedRows = (getPage - 1) * RequestListingModel.ItemPerPage;
            //User can only see own requests
            var query = context.Request.Where(r => string.Compare(r.Username, userName, true) == 0).
                Include(r => r.Response).Include(r => r.CustomerInfo);
            totalRows = query.Count();
            if (asc)
            {
                return query.OrderBy(OrderTranslater(orderBy))
                    .Skip(excludedRows).Take(RequestListingModel
                    .ItemPerPage).ToList();
            }
            return query.OrderByDescending(OrderTranslater(orderBy))
                .Skip(excludedRows)
                .Take(RequestListingModel.ItemPerPage).ToList();
        }
    }
}