using System.Web;

namespace OpenDnsLogs.Services.Session
{
    public class SessionService : ISessionService
    {
        public T GetSessionData<T>(string indexValue) where T : class
        {
            return HttpContext.Current.Session[indexValue] as T;
        }
    }
}