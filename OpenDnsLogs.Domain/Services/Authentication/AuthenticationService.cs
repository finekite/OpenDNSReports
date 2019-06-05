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

        private OpenDNSUserManager userManager;

        public AuthenticationService(IOpenDNSUserManager openDNSUserManager, ILoginService loginService, OpenDNSUserManager userManager)
        {
            this.openDNSUserManager = openDNSUserManager;
            this.loginService = loginService;
            this.userManager = userManager;
        }

        public async Task<string> GetPasswordAsync(IdentityUser user)
        {
            return await userManager.GetPassowrd(user);
        }

        public async Task<ReportResponseDTO> RegisterUser(ReportRequestDTO reportRequest)
        {
           return await openDNSUserManager.RegisterUser(reportRequest);
        }

        public async Task<bool> VerifyOpenDNSLogin(LoginDto loginDto)
        {
            return await loginService.VerifyOpenDNSLogin(loginDto);
        }

        public async Task<bool> VerifyOpenDNSLoginForEmailJob(LoginDto loginDto)
        {
            return await loginService.VerifyOpenDNSLoginForEmailJob(loginDto);
        }
    }
}