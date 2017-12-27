using CashLoanTool.EntityModels;
using CashLoanTool.Jobs.RSA;
using HDB;
using Newtonsoft.Json;
using NLog;
using System;
using System.Text;

namespace CashLoanTool.Jobs
{
    public static class Wrapper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private const string DOBDateFormat = "yyyy-MM-dd";
        private const string RequestTimeFormat = "yyyy-MM-dd'T'hh:mm:ss.fff";

        public static Response DeserializeResponse(string json)
        {
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

        //Substring here to match HDBs requirements
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
                loanNo = rq.LoanNo
                //signature = "xxx"
            };
            hdssRq.signature = CreateSignature(hdssRq); ;
            //logger.Info(hdssRq.requestId);
            //logger.Info(hdssRq.requestTime);
            //logger.Info(hdssRq.requestType);
            //logger.Info(hdssRq.identityCard);
            //logger.Info(hdssRq.gender);
            //logger.Info(hdssRq.address);
            //logger.Info(hdssRq.birthDate);
            //logger.Info(hdssRq.identityCardName);
            //logger.Info(hdssRq.issuePlace);
            //logger.Info(hdssRq.issueDate);
            //logger.Info(hdssRq.phone);
            //logger.Info(hdssRq.loanNo);
            //logger.Info(hdssRq.signature);
            return hdssRq;
        }
        private static string CreateSignature(HDSSRequest hdssRq)
        {
            var concatBuilder = new StringBuilder();
            concatBuilder.Append(hdssRq.requestId).Append(hdssRq.requestTime);
            concatBuilder.Append(hdssRq.requestType).Append(hdssRq.identityCard);
            concatBuilder.Append(hdssRq.identityCardName).Append(hdssRq.phone);
            concatBuilder.Append(hdssRq.loanNo).Append(RSAHelper.Salt);
            //var hash = RSAHelper.Hash($"{hdssRq.requestId}" +
            //    $"{hdssRq.requestTime}{hdssRq.requestType}" +
            //    $"{hdssRq.identityCard}{hdssRq.identityCardName}" +
            //    $"{hdssRq.phone}{hdssRq.loanNo}{RSAHelper.SecretKey}");
            return RSAHelper.SignData(RSAHelper.Hash(concatBuilder.ToString()));
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
