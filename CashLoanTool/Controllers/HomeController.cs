using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CashLoanTool.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using CashLoanTool.EntityModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using CashLoanTool.Filters;
using System.Threading.Tasks;
using CashLoanTool.Helper;
using CashLoanTool.ViewModels;
using System.Linq;
using System.Security.Claims;

namespace CashLoanTool.Helper
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
        public async Task<IActionResult> Index([FromQuery] int page = 1, [FromQuery] string by = "RequestId", [FromQuery] bool asc = false)
        {
            //string role = string.Empty;
            //var claim = HttpContext.User.FindFirst(ClaimTypes.Role);
            //if (claim != null)
            //    role = claim.Value;
            //HttpContext.Response.Cookies.Append("role", role);
            using (_context)
            {
                //var currentUser = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                //default model to inject to view
#if DEBUG
                //This works well, think i use this for convenient
                var model = await RequestListingController.GetModel(_context, SessionStore.ForceGetDevision(this.HttpContext, _context), page, by, asc);
#else
                var model = await RequestListingController.GetModel(_context, SessionStore.ForceGetDevision(this.HttpContext, _context), page, by, asc);
                //var model = await RequestListingController.GetModel(_context, SessionStore.GetDevision(this.HttpContext), page, by, asc);
#endif
                var abilities = from c in this.HttpContext.User.Claims
                          where c.Type == ClaimTypes.Role
                          select c.Value;
                //move this to app init but that will make round trip request to fetch :/ hmmm
                ViewData[nameof(Ability)] = abilities.ToList();
                ViewData[nameof(CityList.Cities)] = CityList.Cities;
                return View(model);
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
