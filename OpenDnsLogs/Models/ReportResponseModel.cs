using OpenDns.Contracts;
using System.ComponentModel.DataAnnotations;

namespace OpenDnsLogs.Models
{
    public class ReportResponseModel
    {
        [Required]
        public ReportResponseDTO ReportResponseDTO { get; set; }
    }
}