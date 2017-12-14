using Quartz;
using System;
using System.Linq;
using HDB;
using CashLoanTool.EntityModels;
using CashLoanTool.Helper;
using NLog;
using Newtonsoft.Json;
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
            logger.Info("Execute job....");
            var schedulerContext = context.Scheduler.Context;
            var conStr = (string)schedulerContext.Get(EnviromentHelper.ConnectionStringKey);
            if (string.IsNullOrEmpty(conStr))
                throw new ArgumentException("Invalid connection string");
            var url = (string)schedulerContext.Get(EnviromentHelper.ApiUrlKey);
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("Invalid API URL");
            try
            {
                using (var dbContext = new CLToolContext(conStr))
                {
                    //Will result in all request, dk why :/
                    //var newRequests3 = dbContext.Request
                    //      .Where(r => !r.HasResponse);
                    //Works
                    //var newRequests2 = dbContext.Request
                    //      .Where(w => !dbContext.Response.Select(s => s.RequestId).Contains(w.RequestId));
                    //Works
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
                        request.RequestSendTime = DateTime.Now;
                        ////Must have customer info at this point
                        //var info = dbContext.CustomerInfo.Where(i => i.RequestId == request.RequestId).Single();
                        var hdssRq = ToHDSSRequest(request, request.CustomerInfo.Single());
                        var result = HDB.Program.PostToHDBank(url, hdssRq);
                        var response = StringToResponse(result);
                        //do smt with response code
                        request.Response.Add(response);
                    }
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                EnviromentHelper.LogException(ex, logger);
                throw;
            }
        }
      
        private const string DOBDateFormat = "yyyy-MM-dd";
        private const string RequestTimeFormat = "yyyy-MM-dd'T'hh:mm:ss.fff";
        private static Response StringToResponse(string json)
        {
            logger.Info("response:");
            logger.Info(json);
            var poco = JsonConvert.DeserializeObject<ResponsePOCO>(json);
            return new Response()
            {
                AcctName = poco.AcctName,
                AcctNo = poco.AcctNo,
                ResponseCode = poco.ResponseCode,
                Signature = poco.Signature,
                ResponseMessage = poco.RespMessage,
                ReceiveTime = DateTime.Now
            };
        }
        public static HDSSRequest ToHDSSRequest(Request rq, CustomerInfo customerInfo)
        {
            var hdssRq = new HDSSRequest()
            {
                //TODO:
                //Home or Contact address is must?
                //interchangeable?
                requestId = Guid.NewGuid().ToString(), //Hardcoded as HDB requested TODO: store this
                requestTime = DateTime.Now.ToString(RequestTimeFormat), //Hardcoded as HDB requested
                requestType = rq.RequestType,
                identityCard = customerInfo.IdentityCard,
                gender = customerInfo.Gender,
                address = customerInfo.HomeAddress,
                birthDate = customerInfo.Dob.ToString(DOBDateFormat),
                identityCardName = customerInfo.FullName, //Full name already strip
                //issuePlace = customerInfo.Issuer, //Indus cant supply this
                issuePlace = "hdsaison",
                issueDate = customerInfo.IssueDate.ToString(DOBDateFormat),
                phone = customerInfo.Phone,
                loanNo = rq.LoanNo,
                signature = "xxx"
            };
            logger.Info("Request:");
            logger.Info(hdssRq.requestId);
            logger.Info(hdssRq.requestTime);
            logger.Info(hdssRq.requestType);
            logger.Info(hdssRq.identityCard);
            logger.Info(hdssRq.gender);
            logger.Info(hdssRq.address);
            logger.Info(hdssRq.birthDate);
            logger.Info(hdssRq.identityCardName);
            logger.Info(hdssRq.issuePlace);
            logger.Info(hdssRq.issueDate);
            logger.Info(hdssRq.phone);
            logger.Info(hdssRq.loanNo);
            logger.Info(hdssRq.signature);

            return hdssRq;
        }

    }
    public class ResponsePOCO
    {
        public string ResponseCode { get; set; }
        public string RespMessage { get; set; }
        public string AcctNo { get; set; }
        public string AcctName { get; set; }
        public string Signature { get; set; }
    }
}