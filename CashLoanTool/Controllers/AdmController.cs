using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CashLoanTool.Const;
using CashLoanTool.EntityModels;
using CashLoanTool.Filters;
using CashLoanTool.Indus;
using CashLoanTool.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog;

namespace CashLoanTool.Helper
{

    [Authorize(Roles = "Admin")]
    [CustomExceptionFilterAttribute] //use to catch unhandle Action Ex
    public class AdmController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private CLToolContext _context;
        private IConfiguration _config;
        private ICustomerAdapter _indus;
        public AdmController(CLToolContext context, IConfiguration config, ICustomerAdapter indusAdapter)
        {
            _context = context;
            _config = config;
            _indus = indusAdapter;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using (_context)
            {
                var model = await GetModel(_context, 1);
                return View(model);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddNewUser([FromBody] CreateUserPost post)
        {
            if(!post.IsValid) return Ok(new ResultWrapper() { Message = "Invalid request", Valid = false });
            var lowerUsername = post.Username.ToLower();

            if (string.IsNullOrEmpty(lowerUsername) || lowerUsername.Length > 50 || Regex.IsMatch(lowerUsername, @"[^0-9a-zA-Z-.]+"))
                return BadRequest();
            var currentUser = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            using (_context)
            {
                if (await _context.User.AnyAsync(u => u.Username == lowerUsername))
                    return Ok(new ResultWrapper() { Message = "Username is already exist!", Valid = false });
                var user = new User()
                {
                    Username = lowerUsername,
                    DivisionName = post.Division,
                    Active = true,
                    Description = $"Created on: {DateTime.Now.ToShortDateString()} | By: {currentUser}",
                    Type = "User"
                };
                if(post.ExportRequests)
                {
                    user.AddAbility(_context, AbilityNames.ExportRequests);
                }
                
                _context.User.Add(user);
                await _context.SaveChangesAsync();
            }
            return Ok(new ResultWrapper() { Message = $"Sucessfully added: {lowerUsername}", Valid = true });
        }
        
        [HttpGet]
        public async Task<IActionResult> FetchModel([FromQuery] int page = 1)
        {
            using (_context)
            {
                return Ok(await GetModel(_context, page));
            }
        }
        private static async Task<AdmViewModel> GetModel(CLToolContext context, int pageNum)
        {
            int getPage = pageNum < 1 ? 1 : pageNum;
            int excludedRows = (getPage - 1) * RequestListingViewModel.ItemPerPage;
            var query = context.User.Include(u => u.UserAbility);
            var model = new AdmViewModel
            {
                Users = await query.OrderBy(u => u.Username)
                             .Skip(excludedRows)
                             .Take(RequestListingViewModel.ItemPerPage).ToListAsync(),
                OnPage = pageNum,
                Divisions = await context.Division.Select(a => a.DivisionName).ToListAsync()
            };
            model.UpdatePagination(await context.User.CountAsync());
            return model;
        }
    }
}
