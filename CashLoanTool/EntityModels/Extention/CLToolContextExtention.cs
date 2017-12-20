using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashLoanTool.EntityModels
{
    public partial class CLToolContext
    {
        //preserve
        public CLToolContext(DbContextOptions<CLToolContext> options) : base(options)
        {
        }
        public CLToolContext(string conStr) : base(new DbContextOptionsBuilder().UseSqlServer(conStr).Options)
        {
        }
        //preserve
    }
}
