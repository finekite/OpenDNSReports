using Microsoft.AspNet.Identity.EntityFramework;
using OpenDns.Contracts;
using System.Threading.Tasks;

namespace OpenDNSAuthorize
{
    public interface IOpenDNSUserManager
    {
        Task<ReportResponseDTO> RegisterUser(ReportRequestDTO reportRequest);

        Task<string> GetPassowrd(IdentityUser identityUser);
    }
}
