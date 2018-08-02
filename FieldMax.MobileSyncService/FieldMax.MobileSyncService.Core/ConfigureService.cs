using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace FieldMax.AsyncService.Core
{
    internal static class ConfigureService
    {
        internal static void Configure()
        {
            HostFactory.Run(configure =>
            {
                configure.Service<CoreService>(service =>
                {
                    service.ConstructUsing(s => new CoreService());
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });
                //Setup Account that window service use to run.  
                configure.RunAsLocalSystem();
                configure.SetServiceName("FieldMax.MobileSyncService");
                configure.SetDisplayName("FieldMax.MobileSyncService");
                configure.SetDescription("Windows service with Topshelf");
            });
        }
    }
}
