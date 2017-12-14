using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CashLoanTool.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using CashLoanTool.EntityModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using CashLoanTool.Filters;

namespace CashLoanTool.Controllers
{
    [CustomExceptionFilterAttribute]
    public class HomeController : Controller
    {
        private CLToolContext _context;
        private IConfiguration _config;
        private IMemoryCache _cache;
        public HomeController(CLToolContext context, IConfiguration config, IMemoryCache memoryCache)
        {
            _context = context;
            _config = config;
            _cache = memoryCache;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index([FromQuery] int page = 1, [FromQuery] string by = "AcctNo", [FromQuery] bool asc = true)
        {
            //string role = string.Empty;
            //var claim = HttpContext.User.FindFirst(ClaimTypes.Role);
            //if (claim != null)
            //    role = claim.Value;
            //HttpContext.Response.Cookies.Append("role", role);
            using (_context)
            {
                var currentUser = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                //default model to inject to view
                var model = RequestListingController.GetModel(_context, currentUser, page, by, asc);
                return View(model);
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
