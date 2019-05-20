using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OpenDns.Contracts
{
    public class LoginDto
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DisplayName("Password")]
        public string Password { get; set; }
    }
}
