﻿using CashLoanTool.EntityModels;
using CashLoanTool.Filters;
using CashLoanTool.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CashLoanTool.Controllers
{
    [CustomExceptionFilterAttribute]
    public class AccountController : Controller
    {
        private const string LoginStatusKey = "LoginStatus";
        //maybe private methods are more suitable since controllers dont seem to get call anywhere in code :/
        internal string Issuer
        {
            get
            {
                return _config.GetSection("Authentication").GetValue<string>("Issuer");
            }
        }
        internal bool NoPwdCheck
        {
            get
            {
                return _config.GetSection("Authentication").GetValue<bool>("NoPwdCheck");
            }
        }

        internal string Domain
        {
            get
            {
                return _config.GetSection("Authentication").GetValue<string>("Domain");
            }
        }
        private CLToolContext _context;
        private IConfiguration _config;
        public AccountController(CLToolContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public enum LoginResult
        {
            Error,
            NotActive,
            NoPermission,
            User,
            ReadOnly,
            Admin
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            HttpContext.Response.Headers.Add("Login", EnviromentHelper.LoginUrl.ToString());
            if (User.Identities.Any(u => u.IsAuthenticated))
            {
                return RedirectToAction("Index", "Home");
            }
            if (TempData.ContainsKey(LoginStatusKey))
                ViewBag.LoginStatus = TempData[LoginStatusKey];
            //return login form view
            ViewBag.NoFooter = true;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> DoLogin([FromForm]string userName = "", [FromForm]string pwd = "")
        {
            var loginLevel = GetLoginLevel(userName, pwd);
            if (loginLevel == LoginResult.Error) return LoginFail();
            if (loginLevel == LoginResult.NoPermission) return NoPermission();
            if (loginLevel == LoginResult.NotActive) return NotActive();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName, ClaimValueTypes.String, Issuer),
                new Claim(ClaimTypes.Role, loginLevel.ToString(), ClaimValueTypes.String, Issuer)
            };
            var userIdentity = new ClaimsIdentity("UserCred");
            userIdentity.AddClaims(claims);
            var userPrincipal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                userPrincipal,
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(60),
                    IsPersistent = false,
                    AllowRefresh = false
                });
            return RedirectToAction("Index", "Home");
        }

        private IActionResult LoginFail()
        {
            TempData[LoginStatusKey] = "Login failed."; //pass data to redirect
            return RedirectToAction("Login", "Account");
        }
        private IActionResult NoPermission()
        {
            TempData[LoginStatusKey] = "No permission found."; //pass data to redirect
            return RedirectToAction("Login", "Account");
        }
        private IActionResult NotActive()
        {
            TempData[LoginStatusKey] = "Account is de-activated."; //pass data to redirect
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Forbidden()
        {
            return View();
        }

        //not proper
        //[DllImport("advapi32.dll")]
        //public static extern bool LogonUser(string userName, string domainName, string password, int LogonType, int LogonProvider, ref IntPtr phToken);
        //private bool ValidateCredentials(string userName, string password)
        //{
        //    IntPtr tokenHandler = IntPtr.Zero;
        //    return LogonUser(userName, Domain, password, 3, 0, ref tokenHandler);
        //}

        private bool ValidateCredentials(string userName, string pwd)
        {
            if (NoPwdCheck) return true;
            using (var pc = new System.DirectoryServices.AccountManagement.PrincipalContext(System.DirectoryServices.AccountManagement.ContextType.Domain, Domain))
            {
                // validate the credentials
                return pc.ValidateCredentials(userName, pwd);
            }
        }
        private LoginResult GetLoginLevel(string userName, string pwd)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(pwd))
                return LoginResult.Error;
            if (!ValidateCredentials(userName, pwd)) return LoginResult.Error;
            using (_context)
            {
                var user = _context.User.FirstOrDefault(u => string.Compare(u.Username, userName) == 0);
                if (user == null)
                    return LoginResult.NoPermission; //no permission

                if (!user.Active)
                    return LoginResult.NotActive;

                var accountType = user.Type;
                if (!Enum.IsDefined(typeof(LoginResult), accountType))
                    return LoginResult.Error;
                return (LoginResult)Enum.Parse(typeof(LoginResult), accountType);
            }
        }

    }
}