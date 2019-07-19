using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenDns.Contracts
{
    public class EmailReportSettings
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string UserId { get; set; }

        public ReportTypes? ReportTypes { get; set; }

        public EmailOccurence? EmailOccurence { get; set; }

        public FromWhen? FromWhen { get; set; }
    }
}
