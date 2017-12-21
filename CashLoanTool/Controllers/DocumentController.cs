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
using System;
using CashLoanTool.ViewModels;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetDocument([FromQuery]string q = "")
        {
            if (string.IsNullOrEmpty(q)) return BadRequest();
            if (!Decode64(q, out int i, out int iss, out int p))
                return BadRequest();
            if (!GetIssuer(iss, p, out var issuer, out var pob))
                return BadRequest();

            using (_context)
            {
                //check valid
                var currentUser = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var request = await _context.Request.Where(r => r.RequestId == i).Include(r => r.CustomerInfo).Include(r => r.Response).FirstOrDefaultAsync();
                //invalid request id
                if (request == null) return BadRequest();
                //user can only print own request
                if (string.Compare(request.Username, currentUser, true) != 0) return Unauthorized();
                //request has no response yet
                if (!request.HasValidAcctNo) return BadRequest();

                var customerInfo = request.CustomerInfo.Single();
                var templatePath = EnviromentHelper.GetDocumentFullPath(TemplateName, DocumentFolder);
                var document = ArgreementMaker.
                    FillTemplate(customerInfo, request.AcctNo, issuer, pob, templatePath);
                var responseStream = new MemoryStream();
                document.Save(responseStream, SaveOptions.PdfDefault);
                //to return file use File()
                return File(responseStream, "application/pdf");

                //Download file
                //return File(responseStream, "application/pdf", $"{request.LoanNo}.pdf");
            }
        }
        private bool Decode64(string base64, out int id, out int iss, out int p)
        {
            id = -1;
            iss = -1;
            p = -1;
            if (string.IsNullOrEmpty(base64)) return false;
            var data = Convert.FromBase64String(base64);
            string decodedString = Encoding.UTF8.GetString(data);
            var splited = decodedString.Split('-');
            //exp 1-2-3
            if (splited.Count() != 3) return false;

            try
            {
                id = int.Parse(splited[0]);
                iss = int.Parse(splited[1]);
                p = int.Parse(splited[2]);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        private bool GetIssuer(int iss, int p, out string issuer, out string pob)
        {
            issuer = string.Empty;
            pob = string.Empty;
            try
            {
                issuer = IssuerList.Issuers[iss];
                pob = IssuerList.Issuers[p];
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }
    }
}
