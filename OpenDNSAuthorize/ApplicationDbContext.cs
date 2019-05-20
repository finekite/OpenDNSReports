using Microsoft.AspNet.Identity.EntityFramework;
using OpenDns.Contracts;
using System.Data.Entity;

namespace OpenDNSAuthorize
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<EmailReportSettings> EmailReportSettings { get; set; }

        public DbSet<EmailReportJob> EmailReportJob { get; set; }
    }
}
