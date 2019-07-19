using OpenDns.Contracts;
using OpenDnsLogs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenDnsLogs.Orchestrators
{
    public interface IHomeOrchestrator
    {
        Task<bool> VerifyOpenDNSLogin(LoginDto loginDto);

        Task<DomainListModel> GenerateReport(ReportRequestDTO reportRequest);

        DomainListModel GetNextSet(DomainListModel domainListModel);

        DomainListModel GetPreviousSet(DomainListModel model);

        Task<ReportResponseDTO> SetUpAccount(ReportRequestDTO reportRequest);

        Task<bool> VerifyOpenDNSLoginNewHttpClient(LoginDto loginDto);

        Task<List<EmailReportSettings>> GetEmailReportSettings(string email);

        Task<string> GetUserId(string email);

        void AddEmailReportSetting(ManageEmailReportsModel reportsModel);

        void EditEmailReportSetting(ManageEmailReportsModel reportsModel);

        void DeleteEmailReportSetting(ManageEmailReportsModel reportsModel);
    }
}
