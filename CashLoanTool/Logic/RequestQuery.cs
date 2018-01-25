using CashLoanTool.EntityModels;
using System.Linq;

namespace CashLoanTool.Logic
{
    public static class RequestsQuery
    {
        public static IQueryable<Request> RequestsByDivision(CLToolContext context, string division)
        {
            return context.Request.Where(r => r.UsernameNavigation.DivisionName == division);
        }
        public static IQueryable<Request> RequestsByUser(CLToolContext context, string userName)
        {
            return context.Request.Where(r => r.Username == userName);
        }
        public static IQueryable<Request> AllRequests(CLToolContext context)
        {
            return context.Request;
        }
    }
}
