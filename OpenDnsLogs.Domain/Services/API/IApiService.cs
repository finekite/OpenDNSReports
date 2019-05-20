using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenDnsLogs.Domain.Services.API
{
    public interface IApiService
    {
        Task<string> GetHtmlAsyncAsString(string url);

        Task<HttpResponseMessage> PostHtmlAsync(FormUrlEncodedContent content, string url);

        Task<Stream> GetHtmlAsyncAsStream(string url);
    }
}
