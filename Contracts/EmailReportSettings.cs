using System.ComponentModel.DataAnnotations;

namespace OpenDns.Contracts
{
    public class EmailReportSettings
    {
        [Key]
        public string UserId { get; set; }

        public ReportTypes? ReportTypes { get; set; }

        public EmailOccurence? EmailOccurence { get; set; }

        public FromWhen? FromWhen { get; set; }
    }
}
