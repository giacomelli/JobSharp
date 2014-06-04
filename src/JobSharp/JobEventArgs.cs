using System;

namespace JobSharp
{
    /// <summary>
    /// Event arguments for events about a job.
    /// </summary>
    public class JobEventArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="JobEventArgs"/> class.
        /// </summary>
        /// <param name="job">The job.</param>
        public JobEventArgs(IJob job)
        {
            Job = job;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the job.
        /// </summary>
        public IJob Job { get; private set; }
        #endregion
    }
}
