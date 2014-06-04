namespace JobSharp
{
    /// <summary>
    /// Define a interface for a job.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Runs the job.
        /// </summary>
        void Run();
    }
}
