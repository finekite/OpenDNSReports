using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenDns.Contracts
{
    public class EmailReportSettings
    {
        [Key, Column(Order = 0)]
        public string UserId { get; set; }

        [Key, Column(Order = 1)]
        public ReportTypes? ReportTypes { get; set; }

        [Key, Column(Order = 2)]
        public EmailOccurence? EmailOccurence { get; set; }

        [Key, Column(Order = 3)]
        public FromWhen? FromWhen { get; set; }
    }
}
