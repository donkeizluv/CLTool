using System;
using System.Net;
using CashLoanTool.EntityModels;
using Oracle.ManagedDataAccess.Client;
using Dapper;
using System.Linq;
using CashLoanTool.Helper;
using NLog;
using System.Data;
using System.IO;
using System.Threading.Tasks;

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

        public async Task<CustomerInfo> GetCustomerInfo(string contractId)
        {
            if (string.IsNullOrEmpty(Query))
                throw new InvalidOperationException("Query is not set");
            if (string.IsNullOrEmpty(contractId))
                throw new ArgumentNullException();
            using (var connection = new OracleConnection())
            {
                connection.ConnectionString = GetConnectionString();
                await connection.OpenAsync();
                var customer =  await GetCustomer(connection, new CommandDefinition(GetQuery(contractId)));
                //fill in input info
                if (customer == null) return null; //Cant find customer
                //Return customer with stripped vietnamese accents
                return StringCleaner.StripAccentsNSpecialChars(customer);
            }
        }
        //TODO: fix this mess
        private async Task<CustomerInfo> GetCustomer(IDbConnection connection, CommandDefinition cmd)
        {
            //Map to object
            var customer = await connection.QueryAsync<CustomerInfo>(cmd);
            if (!customer.Any()) return null;
            var cus = customer.Single();
            if (string.IsNullOrEmpty(cus.Status))
            {
                throw new InvalidDataException("Customer status is null!");
            }
            return cus;
        }
    }
}
