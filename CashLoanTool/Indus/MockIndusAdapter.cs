using System;
using System.Threading.Tasks;
using CashLoanTool.EntityModels;

namespace CashLoanTool.Indus
{
    public class MockIndusAdapter : ICustomerAdapter
    {
        public void Dispose()
        {
            //dispose
        }

        private static Random RND = new Random();
        public async Task<CustomerInfo> GetCustomerInfo(string contractId)
        {
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
                CompanyAddress = "24C PDL..",
                Status = "wgawewe"
            };
        }

        public string GetConnectionString()
        {
            throw new NotImplementedException();
        }
    }
}
