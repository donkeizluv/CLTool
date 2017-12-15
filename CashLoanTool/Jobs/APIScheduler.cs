using CashLoanTool.Helper;
using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashLoanTool.Jobs
{
    public static class APIScheduler
    {
        public static bool Started { get; private set; } = false;
        public static bool Disabled { get; set; } = false;

        public static void StartQuartz(IConfiguration configuration)
        {
            //Check disabled flag
            Disabled = configuration.GetSection("Scheduler").GetValue<bool>("Disabled");
            if (Disabled) return;

            if (Started) throw new InvalidOperationException("Scheduler is already started.");
            Started = true;
            // Grab the Scheduler instance from the Factory 
            var scheduler = StdSchedulerFactory.GetDefaultScheduler();
            //Add context params
            scheduler.Context.Put(EnviromentHelper.ConnectionStringKey, configuration.GetConnectionString("Default"));
            scheduler.Context.Put(EnviromentHelper.ApiUrlKey, configuration.GetSection("API").GetValue<string>("URL"));
            //Create job
            IJobDetail job = JobBuilder.Create<ExternalAPIJob>()
                .WithIdentity("APIJob", "Group1")
                .Build();

            //trigger
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("DefaultTrigger", "Group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(configuration.GetSection("Scheduler").GetValue<int>("Interval"))
                    .RepeatForever())
                .Build();

            //Start sche & job
            scheduler.ScheduleJob(job, trigger);
            scheduler.Start();
        }
    }
}
