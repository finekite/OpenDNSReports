using Microsoft.AspNet.Identity.EntityFramework;
using OpenDns.Contracts;
using OpenDNSAuthorize;
using OpenDnsLogs.Domain.Services.Login;
using System.Threading.Tasks;

namespace OpenDnsLogs.Domain.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private IOpenDNSUserManager openDNSUserManager;

        private ILoginService loginService;


        public AuthenticationService(IOpenDNSUserManager openDNSUserManager, ILoginService loginService)
        {
            this.openDNSUserManager = openDNSUserManager;
            this.loginService = loginService;
        }

        public async Task<string> GetPasswordAsync(IdentityUser user)
        {
            return await openDNSUserManager.GetPassowrd(user);
        }

        public async Task<ReportResponseDTO> RegisterUser(ReportRequestDTO reportRequest)
        {
           return await openDNSUserManager.RegisterUser(reportRequest);
        }

        public async Task<bool> VerifyOpenDNSLogin(LoginDto loginDto)
        {
            return await loginService.VerifyOpenDNSLogin(loginDto);
        }

        public async Task<bool> VerifyOpenDNSLoginNewHttpClient(LoginDto loginDto)
        {
            return await loginService.VerifyOpenDNSLoginNewHttpClient(loginDto);
        }
    }
}