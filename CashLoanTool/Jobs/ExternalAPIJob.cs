using Quartz;
using System;
using System.Linq;
using CashLoanTool.EntityModels;
using CashLoanTool.Helper;
using NLog;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace CashLoanTool.Jobs
{
    //[PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class ExternalAPIJob : IJob
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //Unhandle exception wont crash app
        public void Execute(IJobExecutionContext context)
        {
            //logger.Info("Execute job....");
            try
            {
                var schedulerContext = context.Scheduler.Context;
                var conStr = (string)schedulerContext.Get(EnviromentHelper.ConnectionStringKey);
                if (string.IsNullOrEmpty(conStr))
                    throw new ArgumentException("Invalid connection string");
                var url = (string)schedulerContext.Get(EnviromentHelper.ApiUrlKey);
                if (string.IsNullOrEmpty(url))
                    throw new ArgumentException("Invalid API URL");
                using (var dbContext = new CLToolContext(conStr))
                {
                    //Will result in all request, dk why :/
                    //var newRequests3 = dbContext.Request
                    //      .Where(r => !r.HasResponse);
                    //Works
                    //var newRequests2 = dbContext.Request
                    //      .Where(w => !dbContext.Response.Select(s => s.RequestId).Contains(w.RequestId));
                    //Works
                    //This feels like lazy load but Doc say EF Core doesnt support lazy load?????????
                    var newRequests = dbContext.Request
                          .Where(w => w.Response.Count == 0);

                    Debug.Print(newRequests.Count().ToString());
                    if (newRequests.Count() == 0)
                    {
                        logger.Info("Nothing new...back to sleep Zzzz");
                        return;
                    }
                    logger.Info("New requests count: " + newRequests.Count().ToString());
                    newRequests = newRequests.Include(r => r.CustomerInfo);
                    foreach (var request in newRequests)
                    {
                        //Update rq send time
                        request.RequestSendTime = DateTime.Now;
                        ////Must have customer info at this point
                        var hdssRq = Wrapper.ToHDSSRequest(request, request.CustomerInfo.Single(), out var guid);
                        //If network fail, rq wont get update with response & guid
                        var result = HDB.Program.PostToHDBank(url, hdssRq);
                        var response = Wrapper.DeserializeResponse(result);

                        //Update GUID
                        request.Guid = guid;
                        //Add response to this request
                        request.Response.Add(response);
                        //try save each sucessful API calls
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                EnviromentHelper.LogException(ex, logger);
                logger.Fatal("************** Unhandle exception in Scheduler => Stop scheduler **************");
                var jobEx = new JobExecutionException(ex);
                //Stop all trigger
                jobEx.UnscheduleAllTriggers = true;
                //Apply effect
                throw jobEx;
            }
        }

    }
}