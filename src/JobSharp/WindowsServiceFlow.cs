using System;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Timers;
using HelperSharp;

namespace JobSharp
{
    /// <summary>
    /// Defines the Windows Service's flow.
    /// </summary>
    public sealed class WindowsServiceFlow : IDisposable
    {
        #region Fields
        private readonly Timer m_timer;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsServiceFlow"/> class.
        /// </summary>
        public WindowsServiceFlow()
        {
            m_timer = new Timer(int.Parse(ConfigurationManager.AppSettings["JobSharp:Crontab"]));
            m_timer.Elapsed += (sender, eventArgs) => RunJobs();
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs before a job starts.
        /// </summary>
        public event EventHandler<JobEventArgs> JobStarting;

        /// <summary>
        /// Occurs when a job throw a error.
        /// </summary>
        public event EventHandler<JobErrorEventArgs> JobError;

        /// <summary>
        /// Occurs whe a job ends.
        /// </summary>
        public event EventHandler<JobEventArgs> JobEnded;
        #endregion

        #region Methods
        /// <summary>
        /// Starts the flow.
        /// </summary>
        public void Start()
        {
            JobService.Initialize();
            m_timer.Start();
        }

        /// <summary>
        /// Stops the flow.
        /// </summary>
        public void Stop() 
        { 
            m_timer.Stop(); 
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            m_timer.Dispose();
        }

        /// <summary>
        /// Runs the jobs.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void RunJobs()
        {
            try
            {
                var sw = new Stopwatch();
                var jobsInfo = JobService.GetAllEnabledJobInfos();
                m_timer.Stop();
                var date = DateTime.Now;

                var jobsToExecute = jobsInfo.Where(j => date >= j.NextExecution).ToList();

                if (jobsToExecute.Count > 0)
                {
                    LogService.Write("\n{0}".With(String.Empty.PadRight(80, '-')));
                    LogService.Write("[START] {0} jobs...".With(jobsToExecute.Count));

                    foreach (var jobInfo in jobsToExecute)
                    {
                        sw.Restart();
                        LogService.Write("[JOB START] {0}".With(jobInfo.Name));

                        try
                        {
                            var eventArgs = new JobEventArgs(jobInfo.Job);
                            OnJobStarting(eventArgs);
                            jobInfo.Job.Run();
                            OnJobEnded(eventArgs);
                        }
                        catch (Exception ex)
                        {
                            LogService.WriteError(ex);
                            OnJobError(new JobErrorEventArgs(jobInfo.Job, ex));
                        }

                        sw.Stop();
                        jobInfo.RefreshNextExecution();
                        LogService.Write("[JOB END]");
                        LogService.Write("\tElapsed time: {0} seconds".With(sw.Elapsed.TotalSeconds));
                        LogService.Write("\tNext execution: {0:dd/MM/yy HH:mm:ss}\n".With(jobInfo.NextExecution.ToLocalTime()));
                    }

                    LogService.Write("[END]");
                }
            }
            finally
            {
                m_timer.Start();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:JobError" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="JobErrorEventArgs"/> instance containing the event data.</param>
        private void OnJobError(JobErrorEventArgs eventArgs)
        {
            try
            {
                if (JobError != null)
                {
                    JobError(this, eventArgs);
                }
            }
            catch (Exception ex)
            {
                // Every exception in this point should be just logged to avoid service go down.
                LogService.WriteError(ex);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:JobEnded" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="JobEventArgs"/> instance containing the event data.</param>
        private void OnJobEnded(JobEventArgs eventArgs)
        {
            if (JobEnded != null)
            {
                JobEnded(this, eventArgs);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:JobStarting" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="JobEventArgs"/> instance containing the event data.</param>
        private void OnJobStarting(JobEventArgs eventArgs)
        {
            if (JobStarting != null)
            {
                JobStarting(this, eventArgs);
            }
        }       
        #endregion
    }
}
