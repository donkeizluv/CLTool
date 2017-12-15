using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CashLoanTool.EntityModels;

namespace CashLoanTool.Indus
{
    public class MockIndusAdapter : IIndusAdapter
    {
        public void Dispose()
        {
            //dispose
        }

        private static Random RND = new Random();
        public CustomerInfo GetCustomerInfoIndus(string contractId, out string status)
        {
            status = "Yes";
            return new CustomerInfo()
            {
                ContactAddress = "Somewhere",
                Dob = DateTime.Today,
                FullName = "Luu Nhat Hong",
                Gender = "M",
                MartialStatus = "S",
                HomeAddress = "Right here....",
                IdentityCard = "358487434434",
                IssueDate = DateTime.Today,
                Nationality = "VN",
                Phone = " 354345423434",
                Pob = "342354234434",
                Position = "Officer",
                Professional = "IT",
                CompanyAddress = "24C PDL.."
            };
        }

        public string GetConnectionString()
        {
            throw new NotImplementedException();
        }
    }
}
