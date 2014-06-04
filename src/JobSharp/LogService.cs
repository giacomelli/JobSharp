using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HelperSharp;

namespace JobSharp
{
    /// <summary>
    /// The central point to registers logs.
    /// </summary>
    public static class LogService
    {
        #region Fields
        private static Stopwatch s_stopWatch = new Stopwatch();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the count of errors already registered.
        /// </summary>
        /// <remarks>
        /// Used for functional tests purposes.
        /// </remarks>
        public static long ErrorsCount { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Writes the generic log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        /// <param name="isError">True is the message is about an error.</param>
        public static void Write(string message, bool isError = false)
        {
            if (isError)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(message);
                Skahal.Infrastructure.Framework.Logging.LogService.Error(message);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(message);
                Skahal.Infrastructure.Framework.Logging.LogService.Debug(message);
            }

        }

        /// <summary>
        /// Writes a start log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void WriteStart(string message)
        {
            var msg = "[STARTED] {0}".With(message);
            Write(msg);
            s_stopWatch.Restart();
        }

        /// <summary>
        /// Write a end log.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void WriteEnd(string message)
        {
            s_stopWatch.Stop();

            Write("[ENDED]");
            Write("\tElapsed time: {0} seconds".With(s_stopWatch.Elapsed.TotalSeconds));
            Write("\t{0}{1}".With(message, Environment.NewLine));
        }

        /// <summary>
        /// Writes a error log.
        /// </summary>
        /// <param name="exception">The exception to be writen.</param>
        public static void WriteError(Exception exception)
        {
            ErrorsCount++;
            var message = "[JOB ERROR] {0}\n{1}".With(exception.Message, exception.StackTrace);
            var typeLoadException = exception as ReflectionTypeLoadException;

            if (typeLoadException != null)
            {
                message += string.Join("\n", typeLoadException.LoaderExceptions.Select(m => m.Message));
            }

            Write(message, true);

            WriteInnerExceptions(exception.InnerException);

            Write(message, true);
        }

        /// <summary>
        /// Initializes with the specified log strategy.
        /// </summary>
        /// <param name="logStrategy">The log strategy.</param>
        internal static void Initialize(Skahal.Infrastructure.Framework.Logging.ILogStrategy logStrategy)
        {
            Skahal.Infrastructure.Framework.Logging.LogService.Initialize(logStrategy);
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Writes the inner exceptions.
        /// </summary>
        /// <param name="exception">The exception.</param>
        private static void WriteInnerExceptions(Exception exception)
        {
            if (exception != null)
            {
                Write("\n\n{0}\n{1}".With(exception.Message, exception.StackTrace), true);
                WriteInnerExceptions(exception.InnerException);
            }
        }
        #endregion
    }
}
