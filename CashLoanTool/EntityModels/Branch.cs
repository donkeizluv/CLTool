using System;
using System.Collections.Generic;

namespace CashLoanTool.EntityModels
{
    public partial class Branch
    {
        public Branch()
        {
            Division = new HashSet<Division>();
        }

        public int BranchId { get; set; }
        public string BranchName { get; set; }

        public ICollection<Division> Division { get; set; }
    }
}
