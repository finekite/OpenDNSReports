using OpenDns.Contracts;
using OpenDNSAuthorize;
using OpenDnsLogs.Domain.Services.Authentication;
using OpenDnsLogs.Domain.Services.Email;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void RunEmailJob(FromWhen fromWhen)
        {
            var allAccounts = applicationDbContext.EmailReportSettings.Where(x => x.FromWhen == fromWhen).ToList();
 
            foreach (var item in allAccounts)
            {
               // var whoKnows = applicationDbContext.EmailReportJob.Where(x => x.Id == item.UserId)
            }
        }
    }
}