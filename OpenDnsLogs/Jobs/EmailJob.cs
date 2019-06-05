using Microsoft.AspNet.Identity.EntityFramework;
using OpenDns.Contracts;
using OpenDNSAuthorize;
using OpenDnsLogs.Domain.Services.Authentication;
using OpenDnsLogs.Domain.Services.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OpenDnsLogs.Jobs
{
    public class EmailJob
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

        public async Task<bool> RunEmailJob(FromWhen fromWhen)
        {
            var allAccounts = applicationDbContext.EmailReportSettings.Where(x => x.FromWhen == fromWhen).ToList();

            try
            {
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