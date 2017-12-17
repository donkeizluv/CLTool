using System;
using System.Net;
using CashLoanTool.EntityModels;
using Oracle.ManagedDataAccess.Client;
using System.Text.RegularExpressions;
using Dapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CashLoanTool.Helper;
using NLog;
using System.Data;

namespace CashLoanTool.Indus
{
    public class IndusAdapter : ICustomerAdapter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
            return Query.Replace(paramHolder, StringCleaner.RemoveSpecialChars(contractId));
        }
        public string GetConnectionString()
        {
            return connectionStringTemplate.Replace("{username}", Cred.UserName)
                .Replace("{pwd}", Cred.Password)
                .Replace("{host}", Server)
                .Replace("{port}", Port.ToString())
                .Replace("{sid}", SID);
        }

        public CustomerInfo GetCustomerInfo(string contractId, out string status)
        {
            if (string.IsNullOrEmpty(contractId))
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(Query))
                throw new InvalidOperationException("Query is not set");

            using (var connection = new OracleConnection())
            {
                connection.ConnectionString = GetConnectionString();
                connection.Open();
                var customer =  ToCustomer(connection, new CommandDefinition(GetQuery(contractId)), out status);
                if (customer == null) return null; //Cant find customer
                //Return customer with stripped vietnamese accents
                return StringCleaner.StripAccentsNSpecialChars(customer);
            }
        }
        private CustomerInfo ToCustomer(IDbConnection connection, CommandDefinition cmd, out string status)
        {
            status = string.Empty;
            //TODO: test adding this field as Extention when have time...
            var dyn = connection.Query(cmd.CommandText);
            if (dyn.Count() == 0 || dyn == null) return null;
            status = dyn.Single().Status;
            if (string.IsNullOrEmpty(status))
            {
                throw new InvalidOperationException("Customer status is null or empty!");
            }
            //Map to object
            var customer = connection.Query<CustomerInfo>(cmd);

            if (customer.Count() == 0) return null;
            var cus = customer.Single();
            return cus;
        }
    }
}
