using Microsoft.AspNetCore.Http;
using NLog;
using System;

namespace CashLoanTool.Helper
{
    public static class EnviromentHelper
    {
        //Scheduler exception flag
        public static bool IsSchedulerDown = false;
        public static string EnvStr = string.Empty;
        public static string ConnectionStringKey = "DbConnectionString";
        public static string ApiUrlKey = "HDB_API";
        public static string GetCurrentAssemblyPath(string concat)
        {
            return $"{Program.ExeDir}{concat}";
        }
        
        public static void LogException(Exception ex, Logger logger)
        {
            logger.Error(ex.GetType().ToString());
            logger.Error(ex.Message);
            logger.Error(ex.StackTrace);
            if (ex.InnerException != null)
            {
                logger.Error("Inner Ex:");
                LogException(ex.InnerException, logger);
            }
        }
        public static PathString LoginUrl
        {
            get
            {
                return new PathString("/Account/Login");
            }
        }

    }
}
