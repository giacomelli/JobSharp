using System;
using Skahal.Infrastructure.Framework.Logging;
using Topshelf;
using Topshelf.HostConfigurators;

namespace JobSharp
{
    /// <summary>
    /// The Window Service entry point.
    /// </summary>
    public static class WindowsService
    {
        /// <summary>
        /// Install the Windows Service.
        /// </summary>
        /// <param name="logStrategy">The log strategy that will be used for register the jobs logs..</param>
        /// <param name="configure">The action to configure the Windows Service.</param>
        public static void Install(ILogStrategy logStrategy, Action<HostConfigurator> configure)
        {
            LogService.Initialize(logStrategy);

            HostFactory.Run(x =>
            {
                x.AddCommandLineDefinition(
                "job",
                jobName =>
                {
                    JobService.Initialize();
                    JobService.GetJob(jobName).Run();
                });

                x.Service<WindowsServiceFlow>(
                s =>
                {
                    s.ConstructUsing(f => new WindowsServiceFlow());
                    s.WhenStarted(f => f.Start());
                    s.WhenStopped(f => f.Stop());
                });

                configure(x);
            });
        }
    }
}
