using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CashLoanTool.EntityModels;
using CashLoanTool.Helper;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> GetDocument([FromQuery]string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();
            if (!Decode64(id, out var contractId)) return BadRequest();
            using (_context)
            {
                //check valid
                //var currentUser = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var request = await _context.Request.Where(r => r.RequestId == contractId).Include(r => r.CustomerInfo).Include(r => r.Response).FirstOrDefaultAsync();
                //invalid request id
                if (request == null) return BadRequest();
                //user can only print own request
                //if (string.Compare(request.Username, currentUser, true) != 0) return Unauthorized();
                //request has no response yet
                if (!request.HasValidAcctNo) return BadRequest();

                var customerInfo = request.CustomerInfo.Single();

                //fucked up catcher
                if(string.IsNullOrEmpty(customerInfo.Issuer) || string.IsNullOrEmpty(customerInfo.Pob))
                {
                    return Ok("Khách hàng này thiếu thông tin nơi cấp CMND, nơi sinh => liên hệ luu.nhat-hong@hdsaison.com.vn để bổ sung thông tin");
                }

                var templatePath = EnviromentHelper.GetDocumentFullPath(TemplateName, DocumentFolder);
                var document = ArgreementMaker.
                    FillTemplate(customerInfo, request.AcctNo, templatePath);
                var responseStream = new MemoryStream();
                //document.Save(responseStream, new PdfSaveOptions() { Permissions = PdfPermissions.All });
                //to return file use File()
                ArgreementMaker.AsposePdfStream(document, responseStream);
                return File(responseStream, "application/pdf");

                //Download file
                //return File(responseStream, "application/pdf", $"{request.LoanNo}.pdf");
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> GetDocumentAspose([FromQuery]string q = "")
        //{
        //    if (string.IsNullOrEmpty(q)) return BadRequest();
        //    if (!Decode64(q, out int i, out int iss, out int p))
        //        return BadRequest();
        //    if (!GetIssuer(iss, p, out var issuer, out var pob))
        //        return BadRequest();

        //    using (_context)
        //    {
        //        //check valid
        //        //var currentUser = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
        //        var request = await _context.Request.Where(r => r.RequestId == i).Include(r => r.CustomerInfo).Include(r => r.Response).FirstOrDefaultAsync();
        //        //invalid request id
        //        if (request == null) return BadRequest();
        //        //user can only print own request
        //        //if (string.Compare(request.Username, currentUser, true) != 0) return Unauthorized();
        //        //request has no response yet
        //        if (!request.HasValidAcctNo) return BadRequest();

        //        var customerInfo = request.CustomerInfo.Single();
        //        var templatePath = EnviromentHelper.GetDocumentFullPath(TemplateName, DocumentFolder);
        //        var document = ArgreementMaker.FillTemplateAspose(customerInfo, request.AcctNo, issuer, pob, templatePath);
        //        var responseStream = new MemoryStream();
        //        document.Save(responseStream, SaveFormat.Pdf);
        //        responseStream.Position = 0;
        //        return File(responseStream, "application/pdf");
        //    }
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetDocumentDocx([FromQuery]string q = "")
        //{
        //    if (string.IsNullOrEmpty(q)) return BadRequest();
        //    if (!Decode64(q, out int i, out int iss, out int p))
        //        return BadRequest();
        //    if (!GetIssuer(iss, p, out var issuer, out var pob))
        //        return BadRequest();

        //    using (_context)
        //    {
        //        //check valid
        //        //var currentUser = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
        //        var request = await _context.Request.Where(r => r.RequestId == i).Include(r => r.CustomerInfo).Include(r => r.Response).FirstOrDefaultAsync();
        //        //invalid request id
        //        if (request == null) return BadRequest();
        //        //user can only print own request
        //        //if (string.Compare(request.Username, currentUser, true) != 0) return Unauthorized();
        //        //request has no response yet
        //        if (!request.HasValidAcctNo) return BadRequest();

        //        var customerInfo = request.CustomerInfo.Single();
        //        var templatePath = EnviromentHelper.GetDocumentFullPath(TemplateName, DocumentFolder);
        //        var document = ArgreementMaker.
        //            FillTemplate(customerInfo, request.AcctNo, issuer, pob, templatePath);
        //        var responseStream = new MemoryStream();
        //        document.Save(responseStream, SaveOptions.DocxDefault);
        //        //to return file use File()
        //        return File(responseStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");

        //        //Download file
        //        //return File(responseStream, "application/pdf", $"{request.LoanNo}.pdf");
        //    }
        //}

        private bool Decode64(string base64, out int decoded)
        {
            decoded = -1;
            if (string.IsNullOrEmpty(base64)) return false;
            try
            {
                var data = Convert.FromBase64String(base64);
                string decodedString = Encoding.UTF8.GetString(data);
                decoded = int.Parse(decodedString);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
