using System.Collections.Generic;

namespace OpenDns.Contracts
{
    public class ReportResponseDTO
    {
        public bool Succeeded { get; set; }

        public IEnumerable<string> Messages { get; set; }
    }
}
