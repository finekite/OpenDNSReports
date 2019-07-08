using Microsoft.AspNet.Identity.EntityFramework;
using OpenDns.Contracts;
using System.Threading.Tasks;

namespace OpenDnsLogs.Domain.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<ReportResponseDTO> RegisterUser(ReportRequestDTO reportRequest);

        Task<bool> VerifyOpenDNSLoginNewHttpClient(LoginDto loginDto);

        Task<bool> VerifyOpenDNSLogin(LoginDto loginDto);

        Task<string> GetPasswordAsync(IdentityUser user);
    }
}
