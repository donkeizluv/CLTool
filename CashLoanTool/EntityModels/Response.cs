using System;
using System.Collections.Generic;

namespace CashLoanTool.EntityModels
{
    public partial class Response
    {
        public int ResponseId { get; set; }
        public int RequestId { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string AcctNo { get; set; }
        public string AcctName { get; set; }
        public string Signature { get; set; }
        public DateTime ReceiveTime { get; set; }

        public Request Request { get; set; }
    }
}
