using System;
using System.Collections.Generic;

namespace CashLoanTool.EntityModels
{
    public partial class User
    {
        public User()
        {
            Request = new HashSet<Request>();
            UserAbility = new HashSet<UserAbility>();
        }

        public string Username { get; set; }
        public string Type { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }
        public string DivisionName { get; set; }

        public Division DivisionNameNavigation { get; set; }
        public AccountType TypeNavigation { get; set; }
        public ICollection<Request> Request { get; set; }
        public ICollection<UserAbility> UserAbility { get; set; }
    }
}
