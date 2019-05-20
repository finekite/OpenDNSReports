using OpenDns.Contracts;
using System.Collections.Generic;
using System.Configuration;

namespace OpenDnsLogs.Domain.CustomEnumerations
{
    public class ReportTypeUrls
    {
        public int Id;

        public string Url;

        public ReportTypes ReportType;

        public ReportTypeUrls(int id, string url, ReportTypes reportType)
        {
            Id = id;
            Url = url;
            ReportType = reportType;
        }

        public static ReportTypeUrls AllDomains = new ReportTypeUrls(1, ConfigurationManager.AppSettings["AllDomainsUrl"], ReportTypes.AllWebsites);

        public static ReportTypeUrls TopDomains = new ReportTypeUrls(2, ConfigurationManager.AppSettings["AllDomainsUrl"], ReportTypes.TopWebsites);

        public static ReportTypeUrls BlockedDomains = new ReportTypeUrls(3, ConfigurationManager.AppSettings["BlockedDomainsUrl"], ReportTypes.BlockedWebsites);

        public static List<ReportTypeUrls> AllReportTypeUrls = new List<ReportTypeUrls>
        {
            AllDomains, TopDomains, BlockedDomains
        };
    }
}