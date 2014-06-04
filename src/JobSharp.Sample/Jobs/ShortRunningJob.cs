using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JobSharp.Sample.Jobs
{
    public class ShortRunningJob : IJob
    {
        /// <summary>
        /// Runs the job.
        /// </summary>
        public void Run()
        {
            // Put your job's logic or call your domain logic here.

            // Just for the sample ;).
            Thread.Sleep(1000);
        }
    }
}
