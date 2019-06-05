using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OpenDnsLogs.Domain.Services.API
{
    public class ApiService : IApiService
    {
        private HttpClient httpClient;

        public ApiService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string> GetHtmlAsyncAsString(string url)
        {
            var response = await httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetHtmlAsyncAsStringForEmailJob(string url)
        {
            httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<Stream> GetHtmlAsyncAsStream(string url)
        {
            var response = await httpClient.GetAsync(url);
            var body = await response.Content.ReadAsStreamAsync();
            return body;
        }

        public async Task<HttpResponseMessage> PostHtmlAsync(FormUrlEncodedContent content, string url)
        {
           ClearAndSetHtmlHeaders(httpClient);
           return await httpClient.PostAsync(url, content);
        }

        private void ClearAndSetHtmlHeaders(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        }
    }
}