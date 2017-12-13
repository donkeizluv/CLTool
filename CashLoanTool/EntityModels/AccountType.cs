using System;
using System.Collections.Generic;

namespace CashLoanTool.EntityModels
{
    public partial class AccountType
    {
        public AccountType()
        {
            User = new HashSet<User>();
        }

        public string Type { get; set; }
        public string Description { get; set; }

        public ICollection<User> User { get; set; }
    }
}
