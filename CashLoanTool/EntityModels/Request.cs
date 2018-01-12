using Newtonsoft.Json;
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
        public string LoanNo { get; set; }
        public string Username { get; set; }

        [JsonIgnore]
        public string Guid { get; set; }
        [JsonIgnore]
        public DateTime RequestCreateTime { get; set; }
        [JsonIgnore]
        public DateTime? RequestSendTime { get; set; }
        [JsonIgnore]
        public string RequestType { get; set; }
        [JsonIgnore]
        public string Signature { get; set; }
        [JsonIgnore]
        public User UsernameNavigation { get; set; }
        [JsonIgnore]
        public ICollection<CustomerInfo> CustomerInfo { get; set; }
        [JsonIgnore]
        public ICollection<Response> Response { get; set; }
    }
}
