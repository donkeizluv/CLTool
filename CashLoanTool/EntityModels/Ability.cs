using System;
using System.Collections.Generic;

namespace CashLoanTool.EntityModels
{
    public partial class Ability
    {
        public Ability()
        {
            UserAbility = new HashSet<UserAbility>();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<UserAbility> UserAbility { get; set; }
    }
}
