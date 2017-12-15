using CashLoanTool.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashLoanTool.BussinessRules
{
    public static class CustomerValidator
    {
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
        public static bool Check(CustomerInfo customer, string status, out string message)
        {
            message = string.Empty;
            if (customer == null)
            {
                message = "Không tìm thấy khách hàng.";
                return false;
            }
            //Check status
            if (string.IsNullOrEmpty(status) || string.Compare(status.ToUpper(), AcceptStatus.ToUpper()) != 0)
            {
                message = $"Trạng thái hợp đồng không hợp lệ: {status??string.Empty}";
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
            message = $"Khách hàng hợp lệ. Tên: {customer.FullName}, CMND: {customer.IdentityCard}";
            return true;
        }
    }
}
