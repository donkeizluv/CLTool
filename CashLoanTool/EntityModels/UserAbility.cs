using System;
using System.Collections.Generic;

namespace CashLoanTool.EntityModels
{
    public partial class UserAbility
    {
        public string Ability { get; set; }
        public string Username { get; set; }
        public string Parameter1 { get; set; }
        public string Parameter2 { get; set; }

        public Ability AbilityNavigation { get; set; }
        public User UsernameNavigation { get; set; }
    }
}
