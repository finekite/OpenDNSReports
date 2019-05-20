namespace OpenDnsLogs.Services.Session
{
    public interface ISessionService
    {
        T GetSessionData<T>(string indexValue) where T : class;
    }
}
