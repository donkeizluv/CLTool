using CashLoanTool.EntityModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CashLoanTool.Helper
{
    //session related stuff goes here
    public static class SessionStore
    {
        public static readonly string UserDivisionKey = "Division";
        //Sometimes when server is restarted and all session is clear
        //But user cookie is still authenticated, GetDevision will fail
        //This scenerio is not likely to happen in PROD but causes annoyances in DEV
        //So call this in DEV instead
        public static string ForceGetDevision(HttpContext httpContext, CLToolContext context)
        {
            if (!httpContext.Session.TryGetValue(UserDivisionKey, out var arr))
            {
                SetDivison(httpContext, context);
                return ForceGetDevision(httpContext, context); //try again
            }
            return arr.ConvertToString();
        }
        public static string GetDevision(HttpContext httpContext)
        {
            if (!httpContext.Session.TryGetValue(UserDivisionKey, out var arr))
            {
                throw new InvalidOperationException("Cant get user division despite authenticated context");
            }
            return arr.ConvertToString();
        }
        public static void SetDivison(HttpContext httpContext, CLToolContext context)
        {
            var claim = httpContext.User.FindFirst(ClaimTypes.Name);
            if (claim == null) throw new InvalidOperationException("Cant find user's claim despite authenticated context");
            var user = context.User.FirstOrDefault(u => u.Username == claim.Value);
            if (user == null) throw new InvalidOperationException("No user found in database despite authenticated context");
            SetDivison(httpContext, user);
        }
        public static void SetDivison(HttpContext httpContext, User user)
        {
            httpContext.Session.SetString(UserDivisionKey, user.DivisionName);
        }
    }
}
