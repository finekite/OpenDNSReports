using OpenDns.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenDnsLogs.Domain.Services.Report
{
    public interface IReportService
    {
        Task<List<DomainListDto>> GenerateReport(ReportRequestDTO reportRequest);
    }
}
