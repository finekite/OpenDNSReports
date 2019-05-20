using System;

namespace OpenDnsLogs.Domain.Services.HtmlGenerator
{
    public interface IHtmlBuilder
    {
        string GenerateHtml<TData>(Func<TData, string, string> htmlConstruct, TData obj, string reportType);
    }
}
