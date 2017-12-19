using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CashLoanTool.Indus
{
    public static class IndusFactory
    {
        public static ICustomerAdapter GetIndusInstance(IConfiguration configuration, string sql)
        {
            string server = configuration.GetSection("Indus").GetValue<string>("Server");
            int port = configuration.GetSection("Indus").GetValue<int>("Port");
            string sid = configuration.GetSection("Indus").GetValue<string>("SID");
            string userName = configuration.GetSection("Indus").GetValue<string>("Username");
            string pwd = configuration.GetSection("Indus").GetValue<string>("Pwd");
            var indus = new IndusAdapter(server, port, sid, userName, pwd)
            {
                Query = sql
            };
            return indus;
        }
        public static ICustomerAdapter GetMockInstance()
        {
            return new MockIndusAdapter();
        }
    }
}
