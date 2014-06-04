using Skahal.Infrastructure.Logging.Log4net;
using Topshelf;

namespace JobSharp.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WindowsService.Install(
            new Log4netLogStrategy("JobSharpLog"),
            c =>
            {
                c.RunAsLocalSystem();

                c.SetDescription("A very simple JobSharp Sample");
                c.SetDisplayName("JobSharp Sample");
                c.SetServiceName("JobSharpSample");
                c.StartAutomatically();
                c.EnableShutdown();
            });
        }
    }
}
