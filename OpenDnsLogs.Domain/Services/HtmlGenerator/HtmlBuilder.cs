using System;

namespace OpenDnsLogs.Domain.Services.HtmlGenerator
{
    public class HtmlGenerator : IHtmlBuilder
    {
        public string GenerateHtml<TData>(Func<TData, string, string> htmlConstruct,  TData obj, string reportType)
        {
            return htmlConstruct(obj, reportType);
        }
    }
}