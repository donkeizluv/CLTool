using System;
using System.Net;
using CashLoanTool.EntityModels;
using Oracle.ManagedDataAccess.Client;
using System.Text.RegularExpressions;
using Dapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CashLoanTool.Helper;

namespace CashLoanTool.Indus
{
    public class IndusAdapter : IIndusAdapter
    {
        private const string connectionStringTemplate =
            "User Id={username};Password={pwd};Data Source=(DESCRIPTION=" +
            "(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port}))" +
            "(CONNECT_DATA=(SID={sid})));";

        public int Port { get; private set; }
        public string Server { get; private set; }
        public string SID { get; private set; }
        public string Query { get; set; }
        private const string paramHolder = "{param}";

        public NetworkCredential Cred { get; private set; }
        public IndusAdapter(string server, int port, string sid, string userName, string pwd)
        {
            if (string.IsNullOrEmpty(server)) throw new ArgumentException();
            if (string.IsNullOrEmpty(userName)) throw new ArgumentException();
            if (string.IsNullOrEmpty(sid)) throw new ArgumentException();
            if (string.IsNullOrEmpty(pwd)) throw new ArgumentException();

            Port = port;
            Server = server;
            SID = sid;
            Cred = new NetworkCredential(userName, pwd, "");
        }

        private string GetQuery(string contractId)
        {
            //Strip special chars
            return Query.Replace(paramHolder, EnviromentHelper.RemoveSpecialChars(contractId));
        }
        public string GetConnectionString()
        {
            return connectionStringTemplate.Replace("{username}", Cred.UserName)
                .Replace("{pwd}", Cred.Password)
                .Replace("{host}", Server)
                .Replace("{port}", Port.ToString())
                .Replace("{sid}", SID);
        }

        public CustomerInfo GetCustomerInfoIndus(string contractId)
        {
            if (string.IsNullOrEmpty(Query))
                throw new InvalidOperationException("Query is not set");

            using (var connection = new OracleConnection())
            {
                connection.ConnectionString = GetConnectionString();
                connection.Open();
                var customer =  CustomerConverter.ToCustomer(connection, new CommandDefinition(GetQuery(contractId)));
                if (customer == null) return null; //Cant find customer
                //Return customer with stripped vietnamese accents
                return EnviromentHelper.StripCustomerAccentsNSpecialChars(customer);
            }
        }
    }
}
