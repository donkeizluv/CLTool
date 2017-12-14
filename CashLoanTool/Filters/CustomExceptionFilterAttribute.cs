using CashLoanTool.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashLoanTool.Filters
{
    //Nice!
    //https://stackoverflow.com/questions/38014379/error-handling-in-asp-net-core-1-0-web-api-sending-ex-message-to-the-client
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var logger = LogManager.GetLogger(context.ActionDescriptor.DisplayName);
            EnviromentHelper.LogException(context.Exception, logger);
        }
    }
}
