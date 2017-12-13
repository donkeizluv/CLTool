using System;
using System.Collections.Generic;

namespace CashLoanTool.EntityModels
{
    public partial class Request
    {
        public Request()
        {
            CustomerInfo = new HashSet<CustomerInfo>();
            Response = new HashSet<Response>();
        }

        public int RequestId { get; set; }
        public DateTime RequestCreateTime { get; set; }
        public DateTime? RequestSendTime { get; set; }
        public string RequestType { get; set; }
        public string LoanNo { get; set; }
        public string Signature { get; set; }
        public string Username { get; set; }

        public User UsernameNavigation { get; set; }
        public ICollection<CustomerInfo> CustomerInfo { get; set; }
        public ICollection<Response> Response { get; set; }
    }
}
