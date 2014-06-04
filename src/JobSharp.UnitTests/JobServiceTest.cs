using NUnit.Framework;
using System;
using System.Configuration;
using TestSharp;

namespace JobSharp.UnitTests
{
	[TestFixture ()]
	public class JobServiceTest
	{
		[SetUp ()]
		public void Setup ()
		{
			ConfigurationManager.AppSettings["JobSharp:Jobs"] = "StubTwoJob";
			ConfigurationManager.AppSettings["StubOneJob:Crontab"] = "* * * * *";
			ConfigurationManager.AppSettings["StubTwoJob:Crontab"] = "* * * * *";
			JobService.Initialize ();
		}

		[Test]
		public void GetAllEnabledJobInfos_NoArgs_OneJobEnabled()
		{
			var actual = JobService.GetAllEnabledJobInfos ();
			Assert.AreEqual (1, actual.Count);
			Assert.AreEqual ("StubTwoJob", actual [0].Name);
		}

		[Test]
		public void GetJob_InvalidJob_Exception()
		{
			ExceptionAssert.IsThrowing (new ConfigurationErrorsException ("Job with name 'Test' was not found."), () => {
				JobService.GetJob("Test");
			});
		}

		[Test]
		public void GetJob_ValidJob_Job()
		{
			Assert.AreEqual ("StubOneJob", JobService.GetJob ("StubOneJob").GetType ().Name);
			Assert.AreEqual ("StubTwoJob", JobService.GetJob ("StubTwoJob").GetType ().Name);
		}
	}
}

