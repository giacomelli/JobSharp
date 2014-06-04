using NUnit.Framework;
using System;
using TestSharp;
using System.IO;
using System.Threading;

namespace JobSharp.FunctionalTests
{
	[TestFixture ()]
	public class CommandLineTest
	{
		[SetUp]
		public void SetUp()
		{
			ProcessHelper.KillAll ("JobSharp.Sample");
		}

		[TearDown]
		public void TearDown()
		{
			ProcessHelper.KillAll ("JobSharp.Sample");
		}

		[Test ()]
		public void Run_NoArgs_RunAllJobs ()
		{
			var sampleProjectFolder = VSProjectHelper.GetProjectFolderPath ("JobSharp.Sample");
			var commandLinePath = Path.Combine (sampleProjectFolder, "bin\\Debug\\JobSharp.Sample.exe");
			var logsFolder = Path.Combine (sampleProjectFolder, "bin\\Debug\\Logs");
			var logFilePath = Path.Combine (sampleProjectFolder, "bin\\Debug\\Logs\\log.txt");

			DirectoryHelper.DeleteAllFiles (logsFolder);
			ProcessHelper.Run (commandLinePath, "", false);

			FileHelper.WaitForFileContentContains (logFilePath, "[END]", 120);

			TestSharp.FileAssert.ContainsContent ("[START] 3 jobs", logFilePath);
			TestSharp.FileAssert.ContainsContent ("[JOB START] ShortRunningJob", logFilePath);
			TestSharp.FileAssert.ContainsContent ("Elapsed time", logFilePath);
			TestSharp.FileAssert.ContainsContent ("Next execution", logFilePath);
			TestSharp.FileAssert.ContainsContent ("[JOB START] ProblematicJob", logFilePath);
			TestSharp.FileAssert.ContainsContent ("[JOB ERROR] Houston, we have a problem!", logFilePath);
			TestSharp.FileAssert.ContainsContent ("[JOB START] LongRunningJob", logFilePath);
			TestSharp.FileAssert.ContainsContent ("[JOB END]", logFilePath);
			TestSharp.FileAssert.ContainsContent("[END]", logFilePath);
			TestSharp.FileAssert.IsCountLines(22, logFilePath);
		}
	}
}

