using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OpenDns.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenDNSAuthorize
{
    public class OpenDNSUserManager : IOpenDNSUserManager
    {
        private readonly CustomUserManager userManager;

        public OpenDNSUserManager(CustomUserManager userManager)
        {
            this.userManager = userManager;
        }

        public async Task<ReportResponseDTO> RegisterUser(ReportRequestDTO reportRequest)
        {
            var user = new IdentityUser { UserName = reportRequest.EmailAddress, Email = reportRequest.EmailAddress, EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, reportRequest.Password);
            reportRequest.UserId = user.Id;

            var reportResponse = new ReportResponseDTO
            {
                Succeeded = result.Succeeded,
                Messages = result.Succeeded ? new List<string> () : result.Errors,
            };

            return reportResponse;
        }

        public async Task<string> GetPassowrd(IdentityUser identityUser)
        {
           return await userManager.GetPasswordAsync(identityUser);
        }

        public async Task<string> GetUserId(string email)
        {
            return await userManager.GetUserId(email);
        }
    }
}