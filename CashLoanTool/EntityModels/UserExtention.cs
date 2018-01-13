using CashLoanTool.Const;
using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Collections.Generic;
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
        
        public async Task AddAbility(CLToolContext context, string abilityName)
        {
            var ability = await context.Ability.FirstOrDefaultAsync(a => a.Name == abilityName);
            if (ability == null) throw new InvalidOperationException($"Ablity: {abilityName} doesnt exist!");

            //Find if user currently have ability
            //Incase include includes all UserAbility so we check Username in UserAbility again
            if (await context.UserAbility.AnyAsync(a => a.Ability == abilityName && a.Username == Username))
            {
                //Already have -> ignore
                logger.Info($"Username: {Username} already have {abilityName} => skip");
                return;
            }
            UserAbility.Add(new UserAbility() { Ability = abilityName });
        }
        public async Task RemoveAblity(CLToolContext context, string abilityName)
        {
            //Find
            //Incase include includes all UserAbility so we check Username in UserAbility again
            var userAbility = await context.UserAbility.FirstOrDefaultAsync(a => a.Ability == abilityName && a.Username == Username);
            //Remove if having
            if (userAbility == null)
            {
                //User doesnt have ability
                logger.Info($"Username: {Username} doesnt have {abilityName} => skip");
                return;
            }
            //EF only allow remove on root DbSet
            //UserAbility.Remove(userAbility);
            context.UserAbility.Remove(userAbility);
        }
    }
}
