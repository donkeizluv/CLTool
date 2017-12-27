using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;

namespace CashLoanTool
{
    public class Program
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

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            string appSetting = "appsettings.json";
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseIISIntegration()
                //read json config file
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    IHostingEnvironment env = builderContext.HostingEnvironment;
                    config.AddJsonFile(appSetting, optional: false, reloadOnChange: false);
                })
                .UseStartup<Startup>()
                .Build();
        }
    }
}
