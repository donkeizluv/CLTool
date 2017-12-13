using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using CashLoanTool.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using CashLoanTool.EntityModels;
using Microsoft.EntityFrameworkCore;
using GemBox.Document;
using CashLoanTool.Indus;
using System.Reflection;
using System;
using System.IO;
using Quartz.Impl;
using Quartz;
using CashLoanTool.Jobs;
using System.Collections.Generic;

namespace CashLoanTool
{
    public class Startup
    {
        public static string ExeDir
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        private const string GemboxDocumentKey = "DTJX-2LSB-QJV3-R3XP";
        public Startup(IConfiguration configuration)
        {
            //Register the lib
            ComponentInfo.SetLicense(GemboxDocumentKey);
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //inject im-mem cache
            //services.AddMemoryCache();

            //injecting
            //this will inject context to controller ctor
            services.AddDbContext<CLToolContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("Default")));


            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IIndusAdapter>(CreateIndusInstance());

            //auth service
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                options =>
                {
                    // access inner page w/o cred will get redirected to this
                    options.LoginPath = new PathString("/Account/Login");
                    options.AccessDeniedPath = new PathString("/Account/Forbidden");
                    options.LogoutPath = new PathString("/Account/Logout");
                    options.SlidingExpiration = true; //extend cookie exp as user still on the site
                    //just for fun, cant find a clean way to use this :/
                    //bc url query doesnt play well with form submit in Account/DoLogin
                    options.ReturnUrlParameter = "returnUrl"; 
                });
            //enforce SSL
            //services.Configure<MvcOptions>(options =>
            //{
            //    options.Filters.Add(new RequireHttpsAttribute());
            //});
            //https://github.com/aspnet/Mvc/issues/4842
            services.AddMvc().AddJsonOptions(options =>
            {
                //solve auto camel case prop names
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //ignore loop ref of object contains each other
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        }
        private IIndusAdapter CreateIndusInstance()
        {
            string server = Configuration.GetSection("Indus").GetValue<string>("Server");
            int port = Configuration.GetSection("Indus").GetValue<int>("Port");
            string sid = Configuration.GetSection("Indus").GetValue<string>("SID");
            string userName = Configuration.GetSection("Indus").GetValue<string>("Username");
            string pwd = Configuration.GetSection("Indus").GetValue<string>("Pwd");
            var indus =  new IndusAdapter(server, port, sid, userName, pwd);
            indus.Query = File.ReadAllText($"{ExeDir}\\{Configuration.GetSection("Indus").GetValue<string>("QueryFileName")}");
            return indus;
        }
        private void StartQuartz()
        {
            // Grab the Scheduler instance from the Factory 
            var scheduler = StdSchedulerFactory.GetDefaultScheduler();
            //Add context params
            scheduler.Context.Put(EnviromentHelper.ConnectionStringKey, Configuration.GetConnectionString("Default"));
            scheduler.Context.Put(EnviromentHelper.ApiUrlKey, Configuration.GetSection("API").GetValue<string>("URL"));
            //Create job
            IJobDetail job = JobBuilder.Create<ExternalAPIJob>()
                .WithIdentity("APIJob", "Group1")
                .Build();

            //trigger
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("DefaultTrigger", "Group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(Configuration.GetSection("Scheduler").GetValue<int>("Interval"))
                    .RepeatForever())
                .Build();

            //Start sche & job
            scheduler.ScheduleJob(job, trigger);
            scheduler.Start();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            EnviromentHelper.RootPath = env.ContentRootPath;
            app.UseAuthentication();
            //enforce SSL
            //app.UseRewriter(new RewriteOptions().AddRedirectToHttps((int)HttpStatusCode.Redirect, 44395));

            //Use this in PROD
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseBrowserLink();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //}

            app.UseDeveloperExceptionPage();
            app.UseBrowserLink();
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
                //routes.MapRoute(
                //   name: "api",
                //   template: "api/{controller}/{action}");
            });
            StartQuartz();
        }
    }
}
