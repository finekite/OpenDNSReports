using Microsoft.AspNet.Identity.EntityFramework;
using OpenDns.Contracts;
using OpenDNSAuthorize;
using OpenDnsLogs.Domain.Services.Authentication;
using OpenDnsLogs.Domain.Services.Email;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OpenDnsLogs.Controllers
{
    public class EmailJobController : Controller
    {
        private readonly IEmailService emailService;

        private readonly IAuthenticationService authenticationService;

        private readonly ApplicationDbContext applicationDbContext;

        public EmailJobController(IEmailService emailService, IAuthenticationService authenticationService, ApplicationDbContext applicationDbContext)
        {
            this.emailService = emailService;
            this.authenticationService = authenticationService;
            this.applicationDbContext = applicationDbContext;
        }

        // GET: EmailJob
        public async Task<ActionResult> Index(EmailOccurence emailOccurence)
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
                Log.Information(Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                Log.CloseAndFlush();
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }

            Log.CloseAndFlush();
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
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