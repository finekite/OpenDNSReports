using Microsoft.AspNet.Identity.EntityFramework;
using Ninject;
using OpenDns.Contracts;
using OpenDNSAuthorize;
using OpenDnsLogs.Domain.Services.Authentication;
using OpenDnsLogs.Domain.Services.Email;
using Quartz;
using Quartz.Impl;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDnsLogs.Jobs
{
    public class Scheduler
    {
        public static void StartSchedule(IScheduler scheduler)
        {
            try
            {
                foreach (var fromWhen in Enum.GetValues(typeof(FromWhen)).Cast<FromWhen>())
                {
                    scheduler.Start();

                    var job = JobBuilder.Create<EmailJob>().UsingJobData("FromWhen", (int)fromWhen).Build();
                    var trigger = TriggerBuilder.Create()
                        .WithSchedule(GetCronSchedule(fromWhen))
                        .StartNow()
                        .Build();

                    scheduler.ScheduleJob(job, trigger);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static IScheduleBuilder GetCronSchedule(FromWhen fromWhen)
        {
            switch (fromWhen)
            {
                case FromWhen.LastDay:
                    return CronScheduleBuilder.DailyAtHourAndMinute(12, 1);
                case FromWhen.LastMonth:
                    return CronScheduleBuilder.MonthlyOnDayAndHourAndMinute(1, 12, 1);
                case FromWhen.LastWeek:
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
           var fromWhen = (FromWhen)context.JobDetail.JobDataMap.GetInt("FromWhen");
           return RunEmailJob(fromWhen);
        }

        public async Task<bool> RunEmailJob(FromWhen fromWhen)
        {
            try
            {
                var allAccounts = applicationDbContext.EmailReportSettings.Where(x => x.FromWhen == fromWhen).ToList();

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
            }
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