using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CashLoanTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            string appSetting = "appsettings.json";
#if DEBUG
            appSetting = "appsettings_s.json";

#endif
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
