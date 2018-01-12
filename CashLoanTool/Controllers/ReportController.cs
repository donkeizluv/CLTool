using CashLoanTool.EntityModels;
using CashLoanTool.Filters;
using CashLoanTool.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Threading.Tasks;

namespace CashLoanTool.Helper
{
    [Route("API/[controller]/[action]")]
    [Authorize]
    [CustomExceptionFilterAttribute] //use to catch unhandle Action Ex
    public class ReportController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private CLToolContext _context;
        private IConfiguration _config;
        public ReportController(CLToolContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        [Authorize(Roles = "ExportRequests")]
        public async Task<IActionResult> ExportRequests()
        {
            using (_context)
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM dbo.ExportRequests()";
                    _context.Database.OpenConnection();
                    var reader = await command.ExecuteReaderAsync();
                    var fileName = $"requests_{DateTime.Today.ToString("yyyyMMdd")}.csv";
                    return File(Utility.DataReaderToCsv(reader), "application/octet-stream", fileName);
                }
            }
        }
    }
}
