using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDNSAuthorize
{
    public class CustomUserManager : UserManager<IdentityUser>
    {
        private UserStore<IdentityUser> store;

        public CustomUserManager(UserStore<IdentityUser> store, CustomHasher customHasher) : base(store)
        {
            this.store = store;
            this.PasswordHasher = customHasher;
        }

        public override async Task<IdentityResult> CreateAsync(IdentityUser user, string password)
        {
            var customHasher = this.PasswordHasher as CustomHasher;
            customHasher.userId = user.Id;

            return await base.CreateAsync(user, password);
        }

        public async Task<string> GetPasswordAsync(IdentityUser user)
        {
            var hashedPassword = await store.GetPasswordHashAsync(user);
            return OpenDnsCryptography.Decrypt(hashedPassword, user.Id);
        }
    }
}
