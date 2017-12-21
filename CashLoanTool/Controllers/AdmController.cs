using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CashLoanTool.EntityModels;
using CashLoanTool.Filters;
using CashLoanTool.Indus;
using CashLoanTool.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog;

namespace CashLoanTool.Controllers
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
        public async Task<IActionResult> AddNewUser([FromBody] PostWrapper post)
        {
            var username = post.Post;
            if (string.IsNullOrEmpty(username)|| username.Length > 50 || Regex.IsMatch(username, @"[^0-9a-zA-Z-.]+"))
                return BadRequest();
            var currentUser = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            using (_context)
            {
                if (await _context.User.AnyAsync(u => u.Username == username))
                    return Ok(new ResultWrapper() { Message = "Username is already exist!", Valid = false });
                var user = new User()
                {
                    Username = username,
                    Active = true,
                    Description = $"Created on: {DateTime.Now.ToShortDateString()} | By: {currentUser}",
                    Type = "User"
                };
                _context.User.Add(user);
                await _context.SaveChangesAsync();
            }
            return Ok(new ResultWrapper() { Message = $"Sucessfully added: {username}", Valid = true });
        }
        [HttpGet]
        public async Task<IActionResult> FetchModel([FromQuery] int page = 1)
        {
            using (_context)
            {
                return Ok(await GetModel(_context, page));
            }
        }
        private static async Task<AdmModel> GetModel(CLToolContext context, int pageNum)
        {
            int getPage = pageNum < 1 ? 1 : pageNum;
            int excludedRows = (getPage - 1) * RequestListingModel.ItemPerPage;
            var model = new AdmModel
            {
                Users = await context.User.OrderBy(u => u.Username)
                             .Skip(excludedRows)
                             .Take(RequestListingModel.ItemPerPage).ToListAsync(),
                OnPage = pageNum
            };
            model.UpdatePagination(await context.User.CountAsync());
            return model;
        }
    }
}
