using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashLoanTool.Helper
{
    public class CreateRequestPost
    {
        public string ContractId { get; set; }
        public int IssuePlace { get; set; }
        public int Pob { get; set; }
    }
    public class CreateUserPost
    {
        public string Username { get; set; }
        public string Division { get; set; }
        public bool ExportRequests { get; set; }
        public bool SeeAllRequests { get; set; }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Division);
            }
        }
    }
    public class PostWrapper
    {
        public string Post { get; set; }
    }
    public class ResultWrapper
    {
        public bool Valid { get; set; }
        public string Message { get; set; }
    }
}
