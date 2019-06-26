using Microsoft.AspNet.Identity.EntityFramework;
using Ninject;
using OpenDns.Contracts;
using OpenDNSAuthorize;
using OpenDnsLogs.Domain.Services.Authentication;
using OpenDnsLogs.Domain.Services.Email;
using Quartz;
using Serilog;
using Serilog.Core;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDnsLogs.Jobs
{
    public class Scheduler
    {
        public static void StartSchedule(IScheduler scheduler)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["LogFile"]).CreateLogger();

            try
            {
                foreach (var emailOccurence in Enum.GetValues(typeof(EmailOccurence)).Cast<EmailOccurence>())
                {
                    scheduler.Start();

                    var job = JobBuilder.Create<EmailJob>().UsingJobData("EmailOccurence", (int)emailOccurence).Build();
                    var trigger = TriggerBuilder.Create()
                        .WithSchedule(GetCronSchedule(emailOccurence))
                        .StartNow()
                        .Build();

                    scheduler.ScheduleJob(job, trigger);
                }
            }
            catch (Exception ex)
            {
                Log.Information(Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                //Log.CloseAndFlush();
            }
            Log.CloseAndFlush();
        }

        private static IScheduleBuilder GetCronSchedule(EmailOccurence emailOccurence)
        {
            switch (emailOccurence)
            {
                case EmailOccurence.Daily:
                    return CronScheduleBuilder.DailyAtHourAndMinute(12, 54);
                case EmailOccurence.Monthly:
                    return CronScheduleBuilder.MonthlyOnDayAndHourAndMinute(1, 12, 1);
                case EmailOccurence.Weekly:
                default:
                    return CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(DayOfWeek.Sunday, 12, 1);
            }
        }
    }

    public class EmailJob : IJob
    {
        private readonly IEmailService emailService;

        private readonly IAuthenticationService authenticationService;

        private readonly ApplicationDbContext applicationDbContext; 

        public EmailJob(IEmailService emailService, IAuthenticationService authenticationService, ApplicationDbContext applicationDbContext)
        {
            this.emailService = emailService;
            this.authenticationService = authenticationService;
            this.applicationDbContext = applicationDbContext;
        }


        public Task Execute(IJobExecutionContext context)
        {
           var emailOccurence = (EmailOccurence)context.JobDetail.JobDataMap.GetInt("EmailOccurence");
           return RunEmailJob(emailOccurence);
        }

        public async Task<bool> RunEmailJob(EmailOccurence emailOccurence)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["LogFile"]).CreateLogger();

            try
            {
                var allAccounts = applicationDbContext.EmailReportSettings.Where(x => x.EmailOccurence == emailOccurence).ToList();

                foreach (var item in allAccounts)
                {
                    var user = applicationDbContext.Users.Where(x => x.Id == item.UserId).FirstOrDefault();
                    var password = await authenticationService.GetPasswordAsync(user);

                    if (await authenticationService.VerifyOpenDNSLoginForEmailJob(new LoginDto { UserName = user.Email, Password = password }))
                    { 
                        var reportRequestDTO = ConstructDto(item, user, password);
                        var result = await emailService.SendReport(reportRequestDTO);
                    }
                }
            }
            catch (Exception ex)
            {
                // log here
                Log.Information(Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                Log.CloseAndFlush();
                return false;
            }
            Log.CloseAndFlush();
            return true;
        }

        private ReportRequestDTO ConstructDto(EmailReportSettings emailReportSettings, IdentityUser user, string password)
        {
            return new ReportRequestDTO
            {
                EmailAddress = user.Email,
                FromWhen = emailReportSettings.FromWhen,
                ReportTypes = emailReportSettings.ReportTypes,
                Password = password,
            };
        }
    }
}
