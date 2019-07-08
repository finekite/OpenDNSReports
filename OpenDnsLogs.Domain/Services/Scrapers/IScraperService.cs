using OpenDns.Contracts;
using System.IO;
using System.Threading.Tasks;

namespace OpenDnsLogs.Domain.Services.Scrapers
{
    public interface IScraperService
    {
        Task<string> GetLoginPage(LoginDto loginDto, string token);

        Task<string> GetOpenDnsFormNewHttpClient();

        Task<string> GetOpenDnsForm();

        Task<Stream> GetDomainReportFromOpenDns(string url);
    }
}
