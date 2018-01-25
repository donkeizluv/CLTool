using CashLoanTool.Const;
using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CashLoanTool.EntityModels
{
    public partial class User
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public bool AllowExport
        {
            get
            {
                var ability = UserAbility.FirstOrDefault(a => a.Ability == AbilityNames.ExportRequests);
                return ability != null;
            }
        }
        public bool CanSeeAllRequests
        {
            get
            {
                var ability = UserAbility.FirstOrDefault(a => a.Ability == AbilityNames.SeeAllRequests);
                return ability != null;
            }
        }

        public bool TryAddAbility(CLToolContext context, string abilityName)
        {
            var ability = context.Ability.FirstOrDefault(a => a.Name == abilityName);
            if (ability == null) throw new InvalidOperationException($"Cant find Ablity: {abilityName}");

            //Find if user currently have ability
            //Incase include includes all UserAbility so we check Username in UserAbility again
            if (UserAbility.Any(a => a.Ability == abilityName))
            {
                //Already have -> ignore
                logger.Info($"Username: {Username} already have {abilityName} => skip");
                return false;
            }
            UserAbility.Add(new UserAbility() { AbilityNavigation = ability });
            return true;
        }
        public bool TryRemoveAbility(CLToolContext context, string abilityName)
        {
            //Find
            //Incase include includes all UserAbility so we check Username in UserAbility again
            var userAbility = UserAbility.FirstOrDefault(a => a.Ability == abilityName);
            //Remove if having
            if (userAbility == null)
            {
                //User doesnt have ability
                logger.Info($"Username: {Username} doesnt have {abilityName} => skip");
                return false; 
            }
            //EF only allow remove on root DbSet
            //UserAbility.Remove(userAbility);
            context.UserAbility.Remove(userAbility);
            return true;
        }
    }
}
