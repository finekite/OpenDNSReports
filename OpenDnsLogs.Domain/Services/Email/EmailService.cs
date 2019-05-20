using OpenDns.Contracts;
using OpenDNSAuthorize;
using OpenDnsLogs.Domain.Extensions;
using OpenDnsLogs.Domain.Services.HtmlGenerator;
using OpenDnsLogs.Domain.Services.Report;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace OpenDnsLogs.Domain.Services.Email
{
    public class EmailService : IEmailService
    {
        private const string successMessage = "You have successfully signed up for emails! You will be receiving an email shortly with a sample report. Please look over this report and confirm your satisfaction.";

        private const string errorMessage = "Internal Server Error.Please try again later";

        private readonly ApplicationDbContext applicationDbContext;

        private readonly IReportService reportService;

        private readonly IHtmlBuilder htmlGenerator;

        public EmailService(IReportService reportService, IHtmlBuilder htmlGenerator, ApplicationDbContext applicationDbContext)
        {
            this.reportService = reportService;
            this.htmlGenerator = htmlGenerator;
            this.applicationDbContext = applicationDbContext;
        }

        public async Task<ReportResponseDTO> RegisterEmailReportSettings(ReportRequestDTO reportRequestDTO)
        {
            ReportResponseDTO sendEmailRespoonse;
            applicationDbContext.EmailReportSettings.Add(new EmailReportSettings
            {
                EmailOccurence = reportRequestDTO.EmailOccurence,
                FromWhen = reportRequestDTO.FromWhen,
                ReportTypes = reportRequestDTO.ReportTypes,
                UserId = reportRequestDTO.UserId
            });

            var result = await applicationDbContext.SaveChangesAsync();

            if(result > 0)
            {
                sendEmailRespoonse = await SendReport(reportRequestDTO);
            }
            else
            {
                sendEmailRespoonse = new ReportResponseDTO { Succeeded = false, Messages = new List<string> { errorMessage } };
            }

            return sendEmailRespoonse;
        }

        public async Task<ReportResponseDTO> SendReport(ReportRequestDTO reportRequestDTO)
        {
            var report = await reportService.GenerateReport(reportRequestDTO);
        
            var emailHtml = htmlGenerator.GenerateHtml(BuildHtml, report, reportRequestDTO.ReportTypes.ToName());

            var sendEmailResponse = SendEmail(reportRequestDTO.EmailAddress, emailHtml);

            return sendEmailResponse;
        }

        private ReportResponseDTO SendEmail(string emailAddress, string emailHtml)
        {
            try
            {
                var smtpClient = new SmtpClient
                {
                    Host = "Smtp.Gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(ConfigurationManager.AppSettings["EmailU"], ConfigurationManager.AppSettings["EmailP"])
                };

                smtpClient.Send(new MailMessage
                {
                    IsBodyHtml = true,
                    From = new MailAddress(ConfigurationManager.AppSettings["EmailU"], "No Reply OpenDns Report"),
                    Subject = "Website History Report",
                    Body = emailHtml,
                    BodyEncoding = Encoding.UTF8,
                    To = { emailAddress }
                });

                return new ReportResponseDTO { Succeeded = true, Messages = new List<string> { successMessage } };
            }
            catch (Exception ex)
            {
                return new ReportResponseDTO { Succeeded = false, Messages = new List<string> { errorMessage + Environment.NewLine + ex.Message } };
            }
        }

        private string BuildHtml(List<DomainListDto> domainList, string reportType)
        {
            int count = 0;
            var htmlString = new StringBuilder();
            string fileName = string.Empty;
            string filePath = string.Empty;

            foreach (var item in domainList.Take(20))
            {
                fileName = count % 2 == 0 ? @"~/EmailTableRow.html" : @"~/EmailTableRowWithColor.html";
                filePath = HostingEnvironment.MapPath(fileName);
                htmlString.Append(string.Format(File.ReadAllText(filePath), item.Domain, item.TotalRequests, item.BlockedByCategory));
                count++;
            }

            filePath = HostingEnvironment.MapPath(@"~/EmailTemplate.html");
            return string.Format(File.ReadAllText(filePath), reportType, htmlString);
        }
    }
}