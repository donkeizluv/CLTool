using CashLoanTool.EntityModels;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CashLoanTool.BussinessRules
{
    public static class CustomerValidator
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        //requestId = Guid.NewGuid().ToString(), //Hardcoded as HDB requested TODO: store this
        //requestTime = DateTime.Now.ToString(RequestTimeFormat), //Hardcoded as HDB requested
        //requestType = rq.RequestType,
        //identityCard = customerInfo.IdentityCard,
        //gender = customerInfo.Gender,
        //address = customerInfo.HomeAddress,
        //birthDate = customerInfo.Dob.ToString(DOBDateFormat),
        //identityCardName = customerInfo.FullName, //Full name already strip
        ////issuePlace = customerInfo.Issuer, //Indus cant supply this
        //issuePlace = "hdsaison",
        //issueDate = customerInfo.IssueDate.ToString(DOBDateFormat),
        //phone = customerInfo.Phone,
        //loanNo = rq.LoanNo,
        //signature = "xxx"


        public const string AcceptStatus = "Contract Printing";
        public static bool CheckAndClean(CustomerInfo customer, string contractId, out string message, out CustomerInfo cleaned)
        {
            cleaned = null;
            message = string.Empty;
            if (customer == null)
            {
                message = "Không tìm thấy khách hàng.";
                return false;
            }
            //Check status
            if (string.IsNullOrEmpty(customer.Status) || string.Compare(customer.Status.ToUpper(), AcceptStatus.ToUpper()) != 0)
            {
                message = $"Trạng thái hợp đồng không hợp lệ: {customer.Status ?? string.Empty}";
                return false;
            }
            //CMND
            if (string.IsNullOrEmpty(customer.IdentityCard))
            {
                message = "Không có thông tin CMND khách hàng trên hệ thống!";
                return false;
            }
            //Gender
            if (string.IsNullOrEmpty(customer.Gender))
            {
                message = "Không có thông tin giới tính khách hàng trên hệ thống!";
                return false;
            }
            //Address
            if (string.IsNullOrEmpty(customer.HomeAddress))
            {
                message = "Không có thông tin địa chỉ khách hàng trên hệ thống!";
                return false;
            }
            //DOB
            if (customer.Dob == null)
            {
                message = "Không có thông tin ngày sinh khách hàng trên hệ thống!";
                return false;
            }
            //Name
            if (string.IsNullOrEmpty(customer.FullName))
            {
                message = "Không có thông tin tên khách hàng trên hệ thống!";
                return false;
            }
            //issue date
            if (customer.IssueDate == null)
            {
                message = "Không có thông tin ngày cấp CMND khách hàng trên hệ thống!";
                return false;
            }
            //phone
            if (string.IsNullOrEmpty(customer.Phone))
            {
                message = "Không có thông tin số điện thoại khách hàng trên hệ thống!";
                return false;
            }
            try
            {
                cleaned = TrimFieldLength(customer, contractId);
            }
            catch (InvalidDataException ex)
            {
                message = ex.Message;
                return false;
            }

            message = $"Khách hàng hợp lệ. Tên: {customer.FullName}, CMND: {customer.IdentityCard}";
            return true;
        }
        private const int IdLength = 15;
        private const int IdNameLength = 25;
        private const int AddressLength = 50;
        private const int PhoneLength = 20;
        private const int IssuePlaceLength = 30;
        private const int LoanNoLength = 20;
        public static CustomerInfo TrimFieldLength(CustomerInfo customer, string loanNo)
        {
            //Do not trim ID in anycases!
            if (customer.IdentityCard.Length > IdLength)
            {
                throw new InvalidDataException($"Độ dài CMND({customer.IdentityCard}) lớn hơn qui định: {IdLength}");
            }
            if (customer.FullName.Length > IdNameLength)
            {
                //Trim Fullname
                logger.Info($"Substring FullName: {customer.FullName} => {customer.FullName.Substring(0, IdNameLength)}");
                customer.FullName = customer.FullName.Substring(0, IdNameLength);
            }
            if (customer.HomeAddress.Length > AddressLength)
            {
                //Trim home address
                logger.Info($"Substring HomeAddress: {customer.HomeAddress} => {customer.HomeAddress.Substring(0, AddressLength)}");
                customer.HomeAddress = customer.HomeAddress.Substring(0, AddressLength);
            }
            if (customer.Phone.Length > PhoneLength)
            {
                //Trim phone
                logger.Info($"Substring Phone: {customer.Phone} => {customer.Phone.Substring(0, PhoneLength)}");
                customer.Phone = customer.Phone.Substring(0, PhoneLength);
            }
            //We currently dont supply this
            //if (customer.Issuer.Length > IssuePlaceLength)
            //{
            //    //Trim phone
            //    customer.Issuer = customer.Issuer.Substring(0, IssuePlaceLength);
            //}
            if (loanNo.Length > LoanNoLength)
            {
                throw new InvalidDataException($"Độ dài số HD({loanNo}) lớn hơn qui định: {LoanNoLength}");
            }
            return customer;
        }

    }
}
