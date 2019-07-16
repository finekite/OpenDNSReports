using OpenDns.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenDnsLogs.Models
{
    public class ManageEmailReportsModel
    {
        public List<EmailReportSettings> EmailReportSettingsList { get; set; }

        public EmailReportSettings EmailReportSettings { get; set; }

        public string UserId { get; set; }

        public string Email { get; set; }
    }
}