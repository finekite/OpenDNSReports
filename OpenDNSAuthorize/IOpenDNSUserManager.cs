using OpenDns.Contracts;
using System.Threading.Tasks;

namespace OpenDNSAuthorize
{
    public interface IOpenDNSUserManager
    {
        Task<ReportResponseDTO> RegisterUser(ReportRequestDTO reportRequest);
    }
}
