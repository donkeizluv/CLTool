using CashLoanTool.EntityModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace CashLoanTool.Indus
{
    public static class CustomerConverter
    {
        //throw invalid customer info exception here
        //public static CustomerInfo ToCustomer(IDataReader reader)
        //{
        //    reader.Read();
        //    var customer = new CustomerInfo()
        //    {
        //        FullName = reader.GetString(1),
        //        Dob = reader.GetDateTime(2),
        //        Pob = reader.GetString(3),
        //        IdentityCard = reader.GetString(4),
        //        Nationality = reader.GetString(5),
        //        IssueDate = reader.GetDateTime(6),
        //        Issuer = reader.GetString(7),
        //        Gender = reader.GetBoolean(8),
        //        HomeAddress = reader.GetString(9),
        //        ContactAddress = reader.GetString(10),
        //        Phone = reader.GetString(11),
        //        Professional = reader.GetString(12),
        //        MartialStatus = reader.GetString(13),
        //        //Ignore education
        //        //Ignore GraduateNPostGraduate
        //        CompanyName = reader.GetString(16),
        //        CompanyAddress = reader.GetString(17)
        //    };
        //    return customer;
        //}
        public static CustomerInfo ToCustomer(IDbConnection connection, CommandDefinition cmd)
        {
            var customer = connection.Query<CustomerInfo>(cmd);
            if (customer.Count() == 0) return null;
            var cus = customer.First();
            return cus;
        }
    }
}
