using CashLoanTool.EntityModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CashLoanTool.Indus
{
    public interface ICustomerAdapter
    {
        string GetConnectionString();
        CustomerInfo GetCustomerInfo(string contractId, out string status);
    }
}
