using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CashLoanTool.EntityModels;
using CashLoanTool.Helper;
using System.IO;
using GemBox.Document;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CashLoanTool.DocumentUltility;
using NLog;
using CashLoanTool.Filters;

namespace CashLoanTool.Controllers
{
    [Authorize]
    [CustomExceptionFilterAttribute]
    public class DocumentController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        internal string DocumentFolder
        {
            get
            {
                return _config.GetSection("FileStorage").GetValue<string>("DocumentFolder");
            }
        }
        internal string TemplateName
        {
            get
            {
                return _config.GetSection("FileStorage").GetValue<string>("TemplateName");
            }
        }

        private CLToolContext _context;
        private IConfiguration _config;

        public DocumentController(CLToolContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        public IActionResult GetDocument([FromQuery]int i)
        {
            using (_context)
            {
                //check valid
                var currentUser = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var request = _context.Request.Where(r => r.RequestId == i).Include(r => r.CustomerInfo).Include(r => r.Response).FirstOrDefault();
                //invalid request id
                if (request == null) return BadRequest();
                //user can only print own request
                if (string.Compare(request.Username, currentUser, true) != 0) return Unauthorized();
                //request has no response yet
                if (!request.HasValidAcctNo) return BadRequest();

                var customerInfo = request.CustomerInfo.Single();
                var document = ArgreementMaker.
                    FillTemplate(customerInfo, request.AcctNo, EnviromentHelper.GetDocumentFullPath(TemplateName, DocumentFolder));
                var responseStream = new MemoryStream();
                document.Save(responseStream, SaveOptions.PdfDefault);
                //to return file use File()
                return File(responseStream, "application/pdf");

                //Download file
                //return File(responseStream, "application/pdf", $"{request.LoanNo}.pdf");
            }
        }
    }
}
