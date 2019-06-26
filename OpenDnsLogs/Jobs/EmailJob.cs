using Microsoft.AspNet.Identity.EntityFramework;
using Ninject;
using OpenDns.Contracts;
using OpenDNSAuthorize;
using OpenDnsLogs.Domain.Services.Authentication;
using OpenDnsLogs.Domain.Services.Email;
using Serilog;
using Serilog.Core;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace OpenDnsLogs.Jobs
{
    public class Scheduler
    {
        private readonly IEmailJob emailJob;

        public Scheduler(IEmailJob emailJob)
        {
            this.emailJob = emailJob;
        }

        public void StartSchedule()
        {
            foreach (var emailOccurence in Enum.GetValues(typeof(EmailOccurence)).Cast<EmailOccurence>())
            {
                var timer = new Timer(GetIntervalFromOccurence(emailOccurence));
                timer.Elapsed += (sender, args) => { RunEmailJob(emailOccurence); };
                timer.Enabled = true;
            }
        }

        private void RunEmailJob(EmailOccurence emailOccurence)
        {
            emailJob.RunEmailJob(emailOccurence);
        }

        private static double GetIntervalFromOccurence(EmailOccurence emailOccurence)
        {
            switch (emailOccurence)
            {
                case EmailOccurence.Daily:
                    return GetMiliSecondsFromDates(DateTime.Now, DateTime.Now.AddDays(1));
                case EmailOccurence.Monthly:
                    return 300000;
                case EmailOccurence.Weekly:
                default:
                    return GetMiliSecondsFromDates(DateTime.Now, DateTime.Now.AddDays(7));
            }
        }

        private static double GetMiliSecondsFromDates(DateTime start, DateTime end)
        {
            return (end - start).TotalMilliseconds;
        }
    }

    public class EmailJob : IEmailJob
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
