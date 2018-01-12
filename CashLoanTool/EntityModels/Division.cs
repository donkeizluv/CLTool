using System;
using System.Collections.Generic;

namespace CashLoanTool.EntityModels
{
    public partial class Division
    {
        public Division()
        {
            User = new HashSet<User>();
        }

        public string DivisionName { get; set; }
        public int? BranchId { get; set; }
        public string Description { get; set; }

        public Branch Branch { get; set; }
        public ICollection<User> User { get; set; }
    }
}
