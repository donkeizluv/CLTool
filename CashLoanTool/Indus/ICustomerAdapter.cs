using CashLoanTool.EntityModels;

namespace CashLoanTool.Indus
{
    public interface ICustomerAdapter
    {
        string GetConnectionString();
        CustomerInfo GetCustomerInfo(string contractId);
    }
}
