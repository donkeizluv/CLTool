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
using CashLoanTool.Jobs;


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
            Configuration = configuration;
            //Register the lib
            ComponentInfo.SetLicense(GemboxDocumentKey);
            APIScheduler.StartQuartz(configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //inject im-mem cache
            //services.AddMemoryCache();

            //Inject db context
            services.AddDbContext<CLToolContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("Default")));
            //Inject config
            services.AddSingleton<IConfiguration>(Configuration);
            //Inject indus adapter
            services.AddSingleton<IIndusAdapter>(IndusFactory.GetIndusInstance(Configuration, 
                File.ReadAllText($"{ExeDir}\\{Configuration.GetSection("Indus").GetValue<string>("QueryFileName")}")));

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
        }
    }
}
