using CashLoanTool.EntityModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CashLoanTool.Indus
{
    public interface IIndusAdapter
    {
        string GetConnectionString();
        CustomerInfo GetCustomerInfoIndus(string contractId);
    }
}
