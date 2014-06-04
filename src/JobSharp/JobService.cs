using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HelperSharp;

namespace JobSharp
{
    /// <summary>
    /// The jobs service.
    /// </summary>
    public static class JobService
    {
        #region Fields
        private static bool s_initialized;
        private static List<JobInfo> s_jobsInfo;
        #endregion

        #region Methods
        /// <summary>
        /// Initialize service loading the available jobs.
        /// </summary>
        public static void Initialize()
        {
            if (!s_initialized)
            {
                s_jobsInfo = new List<JobInfo>();

                var availableJobTypes = ReflectionHelper.GetSubclassesOf<IJob>(true);
                var enabledJobNames = GetEnabledJobNames();

                foreach (var jobType in availableJobTypes)
                {
                    Console.WriteLine(jobType);
                    var job = (IJob)Activator.CreateInstance(jobType);
                    var jobInfo = CreateJobInfo(enabledJobNames, job);
                    s_jobsInfo.Add(jobInfo);

                    LogService.Write("[JOB ADD] {0}: {1}".With(jobType.Name, jobInfo.Enabled ? "ENABLED" : "DISABLED"));
                }

                s_jobsInfo = s_jobsInfo.OrderBy(j => j.Order).ToList();
                s_initialized = true;
            }
        }

        /// <summary>
        /// Gets all enabled job infos.
        /// </summary>
        /// <returns>The infos.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IList<JobInfo> GetAllEnabledJobInfos()
        {
            return s_jobsInfo.Where(j => j.Enabled).ToList();
        }

        /// <summary>
        /// Gets the job by name.
        /// </summary>
        /// <param name="jobName">Thge name of the job.</param>
        /// <returns>The job.</returns>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">Job with name '{0}' was not found.</exception>
        public static IJob GetJob(string jobName)
        {
            var jobInfo = s_jobsInfo.FirstOrDefault(j => j.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));

            if (jobInfo == null)
            {
                throw new ConfigurationErrorsException("Job with name '{0}' was not found.".With(jobName));
            }

            return jobInfo.Job;
        }

        /// <summary>
        /// Gets the enabled job names.
        /// </summary>
        /// <returns>The name of jobs.</returns>
        private static string[] GetEnabledJobNames()
        {
            var key = "JobSharp:Jobs";
            var jobs = ConfigurationManager.AppSettings[key];

            if (jobs == null)
            {
                throw new ConfigurationErrorsException("The key '{0}' was not found on the app.config. Please add the key in the app.config file and try again.".With(key));
            }

            var jobNames = jobs.Replace(" ", String.Empty).Split(',');
            return jobNames;
        }

        /// <summary>
        /// Creates the job information.
        /// </summary>
        /// <param name="enabledJobNames">The enabled job names.</param>
        /// <param name="job">The job.</param>
        /// <returns>The job info.</returns>
        private static JobInfo CreateJobInfo(string[] enabledJobNames, IJob job)
        {
            var jobType = job.GetType();
            var jobInfo = new JobInfo(job);

            var jobFromConfig = enabledJobNames
                .Select((name, index) => new { Name = name, Index = index })
                .FirstOrDefault(j => j.Name.Equals(jobType.Name, StringComparison.OrdinalIgnoreCase));

            if (jobFromConfig != null)
            {
                jobInfo.Enabled = true;
                jobInfo.Order = jobFromConfig.Index;
            }

            return jobInfo;
        }
        #endregion
    }
}
