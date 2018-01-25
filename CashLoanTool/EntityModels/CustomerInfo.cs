using System;
using System.Collections.Generic;

namespace CashLoanTool.EntityModels
{
    public partial class CustomerInfo
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime Dob { get; set; }
        public string Pob { get; set; }
        public string IdentityCard { get; set; }
        public DateTime IssueDate { get; set; }
        public string Issuer { get; set; }
        public string Nationality { get; set; }
        public string ContactAddress { get; set; }
        public string HomeAddress { get; set; }
        public string Phone { get; set; }
        public string Professional { get; set; }
        public string Position { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string MartialStatus { get; set; }
        public decimal? LoanAmount { get; set; }
        public int RequestId { get; set; }

        public Request Request { get; set; }
    }
}
