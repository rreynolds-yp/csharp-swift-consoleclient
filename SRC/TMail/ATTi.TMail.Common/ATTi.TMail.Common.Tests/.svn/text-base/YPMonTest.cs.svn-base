using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using ATTi.Core.Trace;
using ATTi.TMail.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ATTi.TMail.Common.Tests
{
    
    
    /// <summary>
    ///This is a test class for YPMonTest and is intended
    ///to contain all YPMonTest Unit Tests
    ///</summary>
	[TestClass()]
	public class YPMonTest
	{

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion

		internal class TestClassType 
		{
			public string name = @"TestClassType!!";
		}

		internal struct TestStructType
		{
			public const string name = @"TestStructType!!";
		}

		internal static class TestStaticClassType
		{
			public static string name = @"TestStaticClassType";
		}

		private static bool VerifyEventEntryFromLog(string expected, string actual, int timeout)
		{
			bool ret = false;

			return ret;
		}

		private static bool VerifyEventEntryFromWinEvent(string expected, string actual, int timeout)
		{
			bool ret = false;

			return ret;
		}

		/// <summary>
		///A test for Warn
		///</summary>
		[TestMethod()]
		public void WarnTest()
		{
			string eventID = string.Empty; // TODO: Initialize to an appropriate value
			string message = string.Empty; // TODO: Initialize to an appropriate value
			YPMon.Warn(eventID, message);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}


		[TestMethod()]
		public void TraceEventCriticalTest() 
		{
			Traceable.TraceEvent(typeof(TestClassType),TraceEventType.Critical, "TraceCritical");
		}

		[TestMethod()]
		public void TraceEventInfoTest()
		{
			Traceable.TraceEvent(typeof(TestClassType), TraceEventType.Information, "TraceInfo");
		}

		[TestMethod()]
		public void TraceEventVerboseTest()
		{
			Traceable.TraceEvent(typeof(TestClassType), TraceEventType.Verbose, "TraceVerbose");
		}

		[TestMethod()]
		public void TraceEventErrorTest()
		{
			Traceable.TraceEvent(typeof(TestClassType), TraceEventType.Error, "TraceError");
		}

		[TestMethod()]
		public void TraceEventWarningTest()
		{
			Traceable.TraceEvent(typeof(TestClassType), TraceEventType.Warning, "TraceWarning");
		}

		/// <summary>
		///A test for Info
		///</summary>
		[TestMethod()]
		public void InfoTest()
		{
			string eventID = string.Empty; // TODO: Initialize to an appropriate value
			string message = string.Empty; // TODO: Initialize to an appropriate value
			YPMon.Info(eventID, message);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Emit
		///</summary>
		[TestMethod()]
		[DeploymentItem("ATTi.TMail.Common.dll")]
		public void EmitTest()
		{
			string eventName = string.Empty; // TODO: Initialize to an appropriate value
			string eventID = string.Empty; // TODO: Initialize to an appropriate value
			string message = string.Empty; // TODO: Initialize to an appropriate value
			string stacktrace = string.Empty; // TODO: Initialize to an appropriate value
			YPMon_Accessor.Emit(eventName, eventID, message, stacktrace);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for DebugIf
		///</summary>
		public void DebugIfTestHelper<T>()
		{
			bool condition = true; // TODO: Initialize to an appropriate value
			string eventID = string.Empty; // TODO: Initialize to an appropriate value
			string message = string.Empty; // TODO: Initialize to an appropriate value
			//bool condition = true;
			YPMon.DebugIf<YPMonTest>(condition, eventID, message);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

	
		[TestMethod()]
		public void DebugIfTest()
		{
			DebugIfTestHelper<GenericParameterHelper>();
		}

		/// <summary>
		///A test for Debug
		///</summary>
		public void DebugTestHelper<T>()
		{
			string eventID = string.Empty; // TODO: Initialize to an appropriate value
			string message = string.Empty; // TODO: Initialize to an appropriate value
			YPMon.Debug<YPMonTest>(eventID, message);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		[TestMethod()]
		public void DebugTest()
		{
			DebugTestHelper<GenericParameterHelper>();
		}

		/// <summary>
		///A test for Critical
		///</summary>
		[TestMethod()]
		public void CriticalTest()
		{
			string eventID = string.Empty; // TODO: Initialize to an appropriate value
			string message = string.Empty; // TODO: Initialize to an appropriate value
			string stacktrace = string.Empty; // TODO: Initialize to an appropriate value
			YPMon.Critical(eventID, message, stacktrace);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}
	}
}
