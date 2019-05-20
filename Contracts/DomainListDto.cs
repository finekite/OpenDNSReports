namespace OpenDns.Contracts
{
    public class DomainListDto
    {
        public string Domain { get; set; }

        public string TotalRequests { get; set; }

        public string BlockedByCategory { get; set; }
    }
}