using System;
using System.Configuration;
using System.Globalization;
using HelperSharp;
using NCrontab;

namespace JobSharp
{
    /// <summary>
    /// Represents informations about a job.
    /// </summary>
    public class JobInfo
    {
        #region Fields
        private CrontabSchedule m_crontab;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="JobInfo"/> class.
        /// </summary>
        /// <param name="job">The job.</param>
        public JobInfo(IJob job)
        {
            ExceptionHelper.ThrowIfNull("job", job);
            Job = job;
            Order = int.MaxValue;

            InitializeCrontab(job);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the job's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the job.
        /// </summary>
        public IJob Job { get; private set; }

        /// <summary>
        /// Gets the next execution.
        /// </summary>
        public DateTime NextExecution { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether job is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the order of execution.
        /// </summary>
        public int Order { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Refresh the next execution date/time indicator.
        /// </summary>
        public void RefreshNextExecution(DateTime? now = null)
        {
            NextExecution = m_crontab.GetNextOccurrence(now.HasValue ? now.Value : DateTime.Now);
        }

        /// <summary>
        /// Verifies if in the day section was used 'L', in this case update to the last month day.
        /// </summary>
        /// <param name="crontabExpression">The original crontab expression.</param>
        /// <returns>The transformed crontab expression.</returns>
        private static string TransfomLastDayOfMonth(string crontabExpression)
        {
            var sections = crontabExpression.Split(' ');
            string daySection;

            if (sections.Length >= 2 && (daySection = sections[2]).Contains("L"))
            {
                daySection = daySection.Replace("L", DateTime.Now.GetEndOfMonth().Day.ToString(CultureInfo.InvariantCulture));
                sections[2] = daySection;

                crontabExpression = String.Join(" ", sections);
            }

            return crontabExpression;
        }

        /// <summary>
        /// Initialize the crontab.
        /// </summary>
        /// <param name="job">The jbo.</param>
        private void InitializeCrontab(IJob job)
        {
            Name = job.GetType().Name;
            CreateCrontab();
            RefreshNextExecution();
        }

        /// <summary>
        /// Create the crontab using the app.config information.
        /// </summary>
        private void CreateCrontab()
        {
            var crontabKey = "{0}:Crontab".With(Name);
            var crontabExpression = ConfigurationManager.AppSettings[crontabKey];

            if (crontabExpression == null)
            {
                throw new ConfigurationErrorsException("The key '{0}' was not found on the app.config. Please add the crontab configuration to the job '{1}' in the app.config file and try again.".With(crontabKey, Name));
            }

            crontabExpression = TransfomLastDayOfMonth(crontabExpression);

            var result = CrontabSchedule.TryParse(crontabExpression);

            if (result.IsError)
            {
                throw new ConfigurationErrorsException("The crontab expression define at key '{0}' on app.config file is invalid. Error: {1}".With(crontabKey, result.Error));
            }

            m_crontab = result.Value;
        }
        #endregion
    }
}
