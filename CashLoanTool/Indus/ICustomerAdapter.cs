using CashLoanTool.EntityModels;
using System.Threading.Tasks;

namespace CashLoanTool.Indus
{
    public interface ICustomerAdapter
    {
        string GetConnectionString();
        Task<CustomerInfo> GetCustomerInfo(string contractId);
        //CustomerInfo GetCustomerInfo(string contractId);
    }
}
