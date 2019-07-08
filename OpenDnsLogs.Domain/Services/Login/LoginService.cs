using OpenDns.Contracts;
using OpenDnsLogs.Domain.Services.Scrapers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenDnsLogs.Domain.Services.Login
{
    public class LoginService : ILoginService
    {
        private readonly string loginFailedMessage = "login failed";

        private readonly string formTokenRegexMatch = @"name\=""formtoken"" value\=""([0-9a-f]*)""";

        private IScraperService scraperService;

        public LoginService(IScraperService scraperService)
        {
            this.scraperService = scraperService;
        }

        public async Task<bool> VerifyOpenDNSLogin(LoginDto loginDto)
        {
            string form = await scraperService.GetOpenDnsForm();
            return await VerifyOpenDNSLoginCommon(loginDto, form);
        }

        public async Task<bool> VerifyOpenDNSLoginNewHttpClient(LoginDto loginDto)
        {
            string form = await scraperService.GetOpenDnsFormNewHttpClient();
            return await VerifyOpenDNSLoginCommon(loginDto, form);
        }

        private async Task<bool> VerifyOpenDNSLoginCommon(LoginDto loginDto, string form)
        {
            if (form.ToLower().Contains(loginDto.UserName))
            {
                return true;
            }

            var loginPage = await scraperService.GetLoginPage(loginDto, ExtractToken(form));
            return !loginPage.ToLower().Contains(loginFailedMessage);
        }

        internal string ExtractToken(string form)
        {
            // Get the token
            var inputTag = Regex.Match(form, formTokenRegexMatch).Value;
            var token = inputTag.Substring(inputTag.LastIndexOf("ue=") + 4);
            token = token.Replace("\\", "").Replace("\"", "");

            return token;
        }
    }
}