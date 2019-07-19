using OpenDns.Contracts;
using OpenDnsLogs.Domain.Services.Authentication;
using OpenDnsLogs.Domain.Services.Email;
using OpenDnsLogs.Domain.Services.Report;
using OpenDnsLogs.Models;
using OpenDnsLogs.Services.Session;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OpenDnsLogs.Orchestrators
{
    public class HomeOrchestrator : IHomeOrchestrator
    {
        private readonly IAuthenticationService authenticationService;

        private readonly IReportService reportService;

        private readonly ISessionService sessionService;

        private readonly IEmailService emailService;

        public HomeOrchestrator(IAuthenticationService authenticationService, IReportService reportService, ISessionService sessionService,
                                IEmailService emailService)
        {
            this.authenticationService = authenticationService;
            this.reportService = reportService;
            this.sessionService = sessionService;
            this.emailService = emailService;
        }

        public async Task<DomainListModel> GenerateReport(ReportRequestDTO reportRequest)
        {
            var domainList = await reportService.GenerateReport(reportRequest);
            HttpContext.Current.Session[reportRequest.ReportTypes.ToString()] = domainList;

            var model = new DomainListModel
            {
                Page = 1,
                AmountPerPage = 15,
                Domains = domainList.Take(15).ToList(),
                ReportType = reportRequest.ReportTypes.Value
            };

            return model;
        }

        public DomainListModel GetPreviousSet(DomainListModel model)
        {
            model.Page--;

            var domainList = sessionService.GetSessionData<List<DomainListDto>>(model.ReportType.ToString());

            var amountToSkip = model.Domains != null ? model.Page * model.AmountPerPage - model.AmountPerPage : domainList.Count - model.AmountPerPage;

            model.Domains = domainList.Skip(amountToSkip).Take(model.AmountPerPage).ToList();

            return model;
        }

        public DomainListModel GetNextSet(DomainListModel model)
        {
            var amountToSkip = model.Page * model.AmountPerPage;

            model.Domains = sessionService.GetSessionData<List<DomainListDto>>(model.ReportType.ToString())
                .Skip(amountToSkip)
                .Take(model.AmountPerPage).ToList();

            model.Page++;

            return model;
        }

        public async Task<ReportResponseDTO> SetUpAccount(ReportRequestDTO reportRequest)
        {
            var authResponse = await authenticationService.RegisterUser(reportRequest);

            if (authResponse.Succeeded)
            {
                authResponse = await emailService.RegisterEmailReportSettings(reportRequest);
            }

            return authResponse;
        }

        public async Task<bool> VerifyOpenDNSLogin(LoginDto loginDto)
        {
            return await authenticationService.VerifyOpenDNSLogin(loginDto);
        }

        public async Task<bool> VerifyOpenDNSLoginNewHttpClient(LoginDto loginDto)
        {
            return await authenticationService.VerifyOpenDNSLoginNewHttpClient(loginDto);
        }

        public async Task<List<EmailReportSettings>> GetEmailReportSettings(string email)
        {
            var userid = await GetUserId(email);

            if (string.IsNullOrEmpty(userid))
            {
                return null;
            }
            return emailService.GetEmailReportSettings(userid);
        }

        public async Task<string> GetUserId(string email)
        {
            return await authenticationService.GetUserId(email);
        }

        public void AddEmailReportSetting(ManageEmailReportsModel reportsModel)
        {
            reportsModel.EmailReportSettings.UserId = reportsModel.UserId;
            emailService.AddReportSetting(reportsModel.EmailReportSettings);
        }

        public void EditEmailReportSetting(ManageEmailReportsModel reportsModel)
        {
            reportsModel.EmailReportSettings.UserId = reportsModel.UserId;
            reportsModel.EmailReportSettings.Id = reportsModel.Id;
            emailService.EditReportSetting(reportsModel.EmailReportSettings);
        }

        public void DeleteEmailReportSetting(ManageEmailReportsModel reportsModel)
        {
            reportsModel.EmailReportSettings.UserId = reportsModel.UserId;
            reportsModel.EmailReportSettings.Id = reportsModel.Id;
            emailService.DeleteReportSetting(reportsModel.EmailReportSettings);
        }
    }
}