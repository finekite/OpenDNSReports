using Ninject;
using OpenDnsLogs.Jobs;
using Quartz;
using Quartz.Simpl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenDnsLogs.App_Start
{
    public class NinjectJobFactory : SimpleJobFactory
    {
        readonly IKernel kernel;
        
        public NinjectJobFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public override IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return this.kernel.Get(bundle.JobDetail.JobType) as IJob;
        }
    }
}