using CashLoanTool.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashLoanTool.EntityModels
{
    public partial class User
    {
        public bool AllowExport
        {
            get
            {
                var ability = UserAbility.FirstOrDefault(a => a.Ability == AbilityNames.ExportRequests);
                return ability != null;
            }
        }
        
        public void AddAbility(CLToolContext context, string abilityName)
        {
            var ability = context.Ability.Single(a => a.Name == abilityName);
            UserAbility.Add(new UserAbility() { Ability = abilityName });
        }
    }
}
