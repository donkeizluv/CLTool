using CashLoanTool.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashLoanTool.API
{
    public interface IHdbApi : IDisposable
    {
        //How to wait for response?????
        bool SendRequest(Request request);

    }
}
