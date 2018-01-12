using Quartz;
using System;
using System.Linq;
using CashLoanTool.EntityModels;
using CashLoanTool.Helper;
using NLog;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CashLoanTool.Jobs
{
    //[PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class ExternalAPIJob : IJob
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //Unhandle exception wont crash app
        //No point using async here...
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
                    var newRequests = dbContext.Request
                          .Where(w => w.Response.Count == 0);

                    //Debug.Print(newRequests.Count().ToString());
                    if (newRequests.Count() == 0)
                    {
                        //logger.Info("Nothing new...back to sleep Zzzz");
                        return;
                    }
                    logger.Info("New requests count: " + newRequests.Count().ToString());
                    newRequests = newRequests.Include(r => r.CustomerInfo);
                    //ToList to close read connection
                    foreach (var request in newRequests.ToList())
                    {
                        //Update rq send time
                        request.RequestSendTime = DateTime.Now;
                        ////Must have customer info at this point
                        var hdssRq = RequestWrapper.ToHDSSRequest(request, request.CustomerInfo.Single());
                        //Log raw rq
                        logger.Info("Request:");
                        logger.Info(JsonConvert.SerializeObject(hdssRq));
                        //var test = JsonConvert.SerializeObject(hdssRq);
                        //If network fail, rq wont get update with response & guid
                        var result = HDB.Program.PostToHDBank(url, hdssRq);
                        var response = RequestWrapper.DeserializeResponse(result);
                        bool skipVerify = false;
                        //00: success
                        //01: có tài khoản cũ rồi nhưng tên sai so với tên đã lưu tại hdb
                        //09: trả về tài khoản cũ
                        //03: tạo tài khoản thất bại, tham số input truyền qua không hợp lệ
                        //05: invalid sig
                        switch (response.ResponseCode)
                        {
                            case "00":
                                //Continue to check sig & save response
                                break;
                            case "09":
                                //Continue to check sig & save response
                                break;
                            case "01":
                                //Doesnt have sig in response => skip verify & store response to check later
                                skipVerify = true;
                                break;
                            case "03":
                                throw new InvalidOperationException($"Server response 03. Invalid format: {response.ResponseMessage}");
                            case "05":
                                throw new InvalidOperationException("Invalid signature 05. Check keys then restart app");
                            default:
                                throw new InvalidOperationException($"Unknown response code {response.ResponseCode}");
                        }

                        if (!skipVerify)
                        {
                            if(!HdbRSA.Verify(response.VerificationHash, response.Signature))
                                throw new UnauthorizedAccessException($"Verification failed for request: {request.RequestId}");
                            logger.Info("Verify OK!");
                        }
                        //Log raw response
                        logger.Info("Response:");
                        logger.Info(result);
                        //Update GUID, sig
                        request.Guid = hdssRq.requestId;
                        request.Signature = hdssRq.signature;
                        //Add response to this request
                        request.Response.Add(response);
                        //try save each sucessful API calls
                        //MultipleActiveResultSets=true;
                        //To allow multiple operations in single connection
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
