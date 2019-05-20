using System;
using System.ComponentModel.DataAnnotations;

namespace OpenDns.Contracts
{
    public class ReportRequestDTO
    {
        [Required(ErrorMessage = "Start Date Required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date Required")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Report Type Required")]
        public ReportTypes? ReportTypes { get; set; }

        [Required(ErrorMessage = "Email Occurence Required")]
        public EmailOccurence? EmailOccurence { get; set; }

        [Required(ErrorMessage = "Required")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "From When Required")]
        public FromWhen? FromWhen { get; set; }

        public string UserId { get; set; }

        public string ActionTaken { get; set; }
    }

    public enum ReportTypes
    {
        [Required]
        [Display(Name = "Top Websites")]
        TopWebsites = 1,

        [Required]
        [Display(Name = "Blocked Websites")]
        BlockedWebsites = 2,

        [Required]
        [Display(Name = "All Websites")]
        AllWebsites = 3,
    }

    public enum FilterTypes
    {
        Default = 1,
        Blocked = 2,
        [Display(Name = "Black Listed")]
        BlackListed = 3,
        Category = 4
    }

    public enum EmailOccurence
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3
    }

    public enum FromWhen
    {
        [Display(Name = "Last Day")]
        LastDay = 1,

        [Display(Name = "Last Week")]
        LastWeek = 2,

        [Display(Name = "Last Month")]
        LastMonth = 3
    }
}
