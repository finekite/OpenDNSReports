using OpenDns.Contracts;
using OpenDnsLogs.Domain.CustomEnumerations;
using OpenDnsLogs.Domain.Services.Scrapers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDnsLogs.Domain.Services.Report
{
    public class ReportService : IReportService
    {
        private readonly IScraperService scraperService;

        public ReportService(IScraperService scraperService)
        {
            this.scraperService = scraperService;
        }

        public async Task<List<DomainListDto>> GenerateReport(ReportRequestDTO reportRequest)
        {
            var dates = ConstructDates(reportRequest);
            var url = string.Format(GetUrlBasedOnReportType(reportRequest.ReportTypes), dates);
            var reportFromOpenDns = await scraperService.GetDomainReportFromOpenDns(url);

            // Read the csv file
            using (var reader = new StreamReader(reportFromOpenDns))
            {
                var domainList = new List<DomainListDto>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    domainList.Add(new DomainListDto
                    {
                        Domain = values[1],
                        TotalRequests = values[2],
                        BlockedByCategory = values[4] == "0" ? "No" : "Yes"
                    });
                }

                if (reportRequest.ReportTypes == ReportTypes.TopWebsites)
                    return domainList.Skip(1).Take(20).ToList();

                return domainList.Skip(1).ToList();
            }
        }

        private string ConstructDates(ReportRequestDTO reportRequest)
        {
            var dateFormat = "yyyy-MM-dd";
            string dates = string.Empty;

            if (reportRequest.EndDate != null && !reportRequest.EndDate.Equals(DateTime.MinValue))
            {
                dates = $@"{reportRequest.StartDate.ToString(dateFormat)}to{reportRequest.EndDate.ToString(dateFormat)}";
            } 
            else if (reportRequest.FromWhen != null)
            {
                dates = BuildDatesFromWhen(reportRequest.FromWhen);
            }
            else
            {
                dates = $@"{reportRequest.StartDate.ToString(dateFormat)}";
            }

            return dates;
        }

        private string BuildDatesFromWhen(FromWhen? fromWhen)
        {
            var today = DateTime.Today.ToShortDateString();

            switch (fromWhen)
            {
                case FromWhen.LastDay:
                    return $"{DateTime.Today.AddDays(-1).ToShortDateString()}to{today}";
                case FromWhen.LastMonth:
                    return $"{DateTime.Today.AddMonths(-1).ToShortDateString()}to{today}";
                default:
                    return $"{DateTime.Today.AddDays(-7).ToShortDateString()}to{today}";
            }
        }

        private string GetUrlBasedOnReportType(ReportTypes? reportType)
        {
            return ReportTypeUrls.AllReportTypeUrls.Where(x => x.ReportType == reportType).FirstOrDefault().Url;
        }
    }
}