using NUnit.Framework;
using System;
using TestSharp;
using Rhino.Mocks;
using System.Configuration;
using HelperSharp;

namespace JobSharp.UnitTests
{
	[TestFixture ()]
	public class JobInfoTest
	{
		[TearDown]
		public void TearDown()
		{
		}

		[Test ()]
		public void Constructor_NullJob_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentNullException ("job"), () => {
				new JobInfo(null);
			});
		}

		[Test ()]
		public void Constructor_NoCrontabKey_Exception ()
		{
			var job = new StubOneJob ();
			var crontabKey = "StubOneJob:Crontab";
			ConfigurationManager.AppSettings[crontabKey] = null;

			ExceptionAssert.IsThrowing (
				new ConfigurationErrorsException("The key '{0}' was not found on the app.config. Please add the crontab configuration to the job 'StubOneJob' in the app.config file and try again.".With(crontabKey)), () => {
				new JobInfo(job);
			});
		}

		[Test ()]
		public void Constructor_InvalidCrontabExpression_Exception ()
		{
			var job = new StubOneJob ();
			var crontabKey = "StubOneJob:Crontab";

			ConfigurationManager.AppSettings[crontabKey] = "X * * * *";
					
			bool thrown = false;

			try
			{
				new JobInfo(job);
			}
			catch(ConfigurationErrorsException ex) {
				thrown = ex.Message.StartsWith ("The crontab expression define at key '{0}' on app.config file is invalid. Error:".With(crontabKey));
			}

			Assert.IsTrue (thrown, "Exception was not thrown");
		}

		[Test ()]
		public void Constructor_LastDayOfMonthMark_NextExecutionInLastDayOfMonth ()
		{
			var job = new StubOneJob ();
			var crontabKey = "StubOneJob:Crontab";

			ConfigurationManager.AppSettings[crontabKey] = "0 0 L * *";

			var target = new JobInfo(job);
			var actual = target.NextExecution;

			Assert.AreEqual (DateTime.Now.GetEndOfMonth ().Day, actual.Day);
		}

		[Test ()]
		public void RefreshNextExecution_CronttabExpression_NextDateTimeExecution ()
		{
			var job = new StubOneJob ();
			var crontabKey = "StubOneJob:Crontab";
			ConfigurationManager.AppSettings[crontabKey] = "* * * * *";


			var target = new JobInfo (job);
			var actual = target.NextExecution;

			target.RefreshNextExecution (DateTime.Now.AddMinutes (1));
			Assert.IsTrue (actual < target.NextExecution);

			actual = target.NextExecution;
			Assert.IsTrue (actual == target.NextExecution);

			target.RefreshNextExecution (DateTime.Now.AddMinutes (2));
			Assert.IsTrue (actual < target.NextExecution);
		}
	}
}

