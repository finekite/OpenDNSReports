using OpenDns.Contracts;
using System.Collections.Generic;

namespace OpenDnsLogs.Models
{
    public class DomainListModel
    {
        public int Page { get; set; }

        public int AmountPerPage { get; set; }

        public ReportTypes? ReportType { get; set; }

        public List<DomainListDto> Domains { get; set; }
    }
}