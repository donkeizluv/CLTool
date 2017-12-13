using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CashLoanTool.EntityModels;

namespace CashLoanTool.API
{
    public class MockApi : IHdbApi
    {
        public void Dispose()
        {
            //dispose
        }
        private Random RND = new Random();
        public bool SendRequest(Request request)
        {
            //emulate 50 50 chance 
            return (RND.Next(0, 100) >= 50);
        }
    }
}
