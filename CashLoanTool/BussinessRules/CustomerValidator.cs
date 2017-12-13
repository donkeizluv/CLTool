using CashLoanTool.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashLoanTool.BussinessRules
{
    public static class CustomerValidator
    {
        public static bool Check(CustomerInfo customer, out string message)
        {
            message = string.Empty;

            if(customer == null)
            {
                message = "Không tìm thấy khách hàng.";
                return false;
            }
            return true;
        }
    }
}
