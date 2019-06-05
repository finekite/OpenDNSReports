using OpenDns.Contracts;
using System.Threading.Tasks;

namespace OpenDnsLogs.Domain.Services.Login
{
    public interface ILoginService
    {
        Task<bool> VerifyOpenDNSLogin(LoginDto loginDto);

        Task<bool> VerifyOpenDNSLoginForEmailJob(LoginDto loginDto);
    }
}
