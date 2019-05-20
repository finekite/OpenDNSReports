using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Ninject;

namespace OpenDNSAuthorize
{
    public class OpenDNSDependencyResolver
    {
        public void ResolveDependencies(IKernel kernel)
        {
            try
            {

                kernel.Bind<UserStore<IdentityUser>>().ToSelf().InSingletonScope();
                kernel.Bind<CustomHasher>().ToSelf().InSingletonScope();
                kernel.Bind<UserManager<IdentityUser>>().ToSelf().InSingletonScope();
                kernel.Bind<CustomUserManager>().ToSelf().InSingletonScope();
                kernel.Bind<IOpenDNSUserManager>().To<OpenDNSUserManager>().InSingletonScope();
            }
            catch
            {

            }
        }
    }
}
