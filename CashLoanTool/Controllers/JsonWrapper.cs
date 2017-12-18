using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashLoanTool.Controllers
{
    public class PostWrapper
    {
        public string Post { get; set; }
    }
    public class ResultWrapper
    {
        public bool Valid { get; set; }
        public string Message { get; set; }
    }
}
