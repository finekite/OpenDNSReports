using OpenDns.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDnsLogs.Jobs
{
    public interface IEmailJob
    {
        Task<bool> RunEmailJob(EmailOccurence emailOccurence);
    }
}
