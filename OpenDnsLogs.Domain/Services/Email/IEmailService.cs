using OpenDns.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenDnsLogs.Domain.Services.Email
{
    public interface IEmailService
    {
        Task<ReportResponseDTO> RegisterEmailReportSettings(ReportRequestDTO reportRequestDTO);

        Task<ReportResponseDTO> SendReport(ReportRequestDTO reportRequestDTO);

        List<EmailReportSettings> GetEmailReportSettings(string userid);

        void AddReportSetting(EmailReportSettings emailReportSettings);
    }
}
