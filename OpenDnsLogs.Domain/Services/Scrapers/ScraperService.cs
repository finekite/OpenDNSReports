using OpenDns.Contracts;
using OpenDnsLogs.Domain.Services.API;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenDnsLogs.Domain.Services.Scrapers
{
    public class ScraperService : IScraperService
    {
        private readonly IApiService apiService;

        public ScraperService(IApiService apiService)
        {
            this.apiService = apiService;
        }

        public async Task<string> GetOpenDnsForm()
        {
            // They tend to switch between these two urls so keep them both
            //https://dashboard.opendns.com
            //https://login.opendns.com/?source=dashboard

            return await apiService.GetHtmlAsyncAsString(ConfigurationManager.AppSettings["OpenDNSDashboardUrl"]);
        }

        public async Task<string> GetOpenDnsFormNewHttpClient()
        {
            // They tend to switch between these two urls so keep them both
            //https://dashboard.opendns.com
            //https://login.opendns.com/?source=dashboard

            return await apiService.GetHtmlAsyncNewHttpClient(ConfigurationManager.AppSettings["OpenDNSDashboardUrl"]);
        }

        public async Task<string> GetLoginPage(LoginDto loginDto, string token)
        {
            // Add creds
            var creds = new Dictionary<string, string>();
            creds.Add("formtoken", token);
            creds.Add("username", loginDto.UserName);
            creds.Add("password", loginDto.Password);
            var content = new FormUrlEncodedContent(creds);

            // Get the "logging you in...." page
            var details = await apiService.PostHtmlAsync(content, ConfigurationManager.AppSettings["OpenDNSLoginUrl"]);

            return details.Content.ReadAsStringAsync().Result;
        }

        public async Task<Stream> GetDomainReportFromOpenDns(string url)
        {
            return await apiService.GetHtmlAsyncAsStream(url);
        }
    }
}