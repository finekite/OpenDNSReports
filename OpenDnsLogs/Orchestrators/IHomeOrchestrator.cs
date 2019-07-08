using OpenDns.Contracts;
using OpenDnsLogs.Models;
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
    }
}
