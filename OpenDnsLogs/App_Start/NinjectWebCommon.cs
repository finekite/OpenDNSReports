[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(OpenDnsLogs.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(OpenDnsLogs.App_Start.NinjectWebCommon), "Stop")]

namespace OpenDnsLogs.App_Start
{
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using OpenDNSAuthorize;
    using OpenDnsLogs.Domain.Services.API;
    using OpenDnsLogs.Domain.Services.Authentication;
    using OpenDnsLogs.Domain.Services.Email;
    using OpenDnsLogs.Domain.Services.HtmlGenerator;
    using OpenDnsLogs.Domain.Services.Login;
    using OpenDnsLogs.Domain.Services.Report;
    using OpenDnsLogs.Domain.Services.Scrapers;
    using OpenDnsLogs.Jobs;
    using OpenDnsLogs.Orchestrators;
    using OpenDnsLogs.Services.Session;
    using System;
    using System.Net.Http;
    using System.Web;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                RegisterServices(kernel);
                StartServices(kernel);
                return kernel;
            }
            catch (Exception ex)
            {
                kernel.Dispose();
                throw;
            }
        }

        private static void StartServices(StandardKernel kernel)
        {
            kernel.Get<Scheduler>().StartSchedule();
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel) 
        {
            kernel.Bind<HttpClient>().To<HttpClient>().InSingletonScope();
            kernel.Bind<IApiService>().To<ApiService>().InSingletonScope();
            kernel.Bind<IScraperService>().To<ScraperService>().InSingletonScope();
            kernel.Bind<ILoginService>().To<LoginService>().InSingletonScope();
            kernel.Bind<IReportService>().To<ReportService>().InSingletonScope();
            kernel.Bind<ISessionService>().To<SessionService>().InSingletonScope();
            kernel.Get<OpenDNSDependencyResolver>().ResolveDependencies(kernel);
            kernel.Bind<IAuthenticationService>().To<AuthenticationService>().InSingletonScope();
            kernel.Bind<IEmailService>().To<EmailService>().InSingletonScope();
            kernel.Bind<IHtmlBuilder>().To<HtmlGenerator>().InSingletonScope();
            kernel.Bind<IHomeOrchestrator>().To<HomeOrchestrator>().InSingletonScope();
            kernel.Bind<IEmailJob>().To<EmailJob>().InSingletonScope();
            kernel.Bind<Scheduler>().ToSelf().InSingletonScope();
        }
    }
}
