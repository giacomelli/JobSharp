using System;

namespace JobSharp
{
    /// <summary>
    /// Event arguments for events about an erro in a job.
    /// </summary>
    public class JobErrorEventArgs : JobEventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="JobErrorEventArgs"/> class.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <param name="exception">The exception.</param>
        public JobErrorEventArgs(IJob job, Exception exception)
            : base(job)
        {
            Exception = exception;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the exception.
        /// </summary>
        public Exception Exception { get; private set; }
        #endregion
    }
}
