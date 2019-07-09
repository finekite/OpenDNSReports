using Microsoft.Extensions.Caching.Memory;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace OpenDnsLogs.Domain.Services.API
{
    public class ApiService : IApiService
    {
        private IMemoryCache memoryCache;

        private HttpClient httpClient;

        public ApiService(IMemoryCache memoryCache, HttpClient httpClient)
        {
            this.memoryCache = memoryCache;
            this.httpClient = httpClient;
        }

        public async Task<string> GetHtmlAsyncAsString(string url)
        {
            TryGetMemoryCacheHttpClient(out httpClient);
            var response = await httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetHtmlAsyncNewHttpClient(string url)
        {
            TryGetMemoryCacheHttpClient(out httpClient);

            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                // generate guid for cookie token
                var guid = Guid.NewGuid().ToString();

                // add cookie with token
                var cookie = new HttpCookie("Token", guid);
                cookie.Expires = DateTime.Now.AddMinutes(15);
                HttpContext.Current.Response.Cookies.Clear();
                HttpContext.Current.Request.Cookies.Clear();
                HttpContext.Current.Response.Cookies.Add(cookie);
                HttpContext.Current.Request.Cookies.Add(cookie);

                // add token and httpclient to memory cache to mantain session for user 
                memoryCache.Set(guid, httpClient); 
            }

            // return response
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<Stream> GetHtmlAsyncAsStream(string url)
        {
            TryGetMemoryCacheHttpClient(out httpClient);
            var response = await httpClient.GetAsync(url);
            var body = await response.Content.ReadAsStreamAsync();
            return body;
        }

        public async Task<HttpResponseMessage> PostHtmlAsync(FormUrlEncodedContent content, string url)
        {
           TryGetMemoryCacheHttpClient(out httpClient);
           ClearAndSetHtmlHeaders(httpClient);
           return await httpClient.PostAsync(url, content);
        }

        private void ClearAndSetHtmlHeaders(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        }

        private void TryGetMemoryCacheHttpClient(out HttpClient httpClient)
        {
            HttpClient cachedHttpClient;

            HttpCookie tokenCookie = HttpContext.Current.Request.Cookies["Token"];

            if (tokenCookie != null)
            {
                cachedHttpClient = memoryCache.Get<HttpClient>(tokenCookie.Value);

                if (cachedHttpClient == null)
                {
                    cachedHttpClient = new HttpClient();
                }
            }
            else
            {
                cachedHttpClient = new HttpClient();
            }

            httpClient = cachedHttpClient;
        }

    }
}