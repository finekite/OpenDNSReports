using OpenDns.Contracts;

namespace OpenDnsLogs.Models
{
    public class LoginModel
    {
        public LoginDto Login { get; set; }

        private string errorMessage;

        public string ErrorMessage
        {
            get
            {
                var mesg = this.errorMessage ?? "";
                return mesg;
            }
            set
            {
                this.errorMessage = value;
            }
        }
    }
}