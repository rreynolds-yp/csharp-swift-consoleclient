using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ATTi.Core.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ATTi.Core.Tests
{
	[TestClass]
	public class PreconditionTests
	{
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		[TestMethod]
		[ExpectedException(typeof(PreconditionContractException))]
		public void Ensure_IsNotNullThrowsExceptionWhenValueGivenIsNull()
		{
			Require.IsNotNull<object>(null);
			Assert.Fail("should have blown up on null value");
		}

		[TestMethod]
		[ExpectedException(typeof(PreconditionContractException))]
		public void Ensure_IsNotNullThrowsExceptionWhenNamedValueGivenIsNull()
		{
			Require.IsNotNull<object>("testValue", null);
		}

		[TestMethod]
		[ExpectedException(typeof(PreconditionContractException))]
		public void Ensure_IsNotNullThrowsExceptionWhenNamedValueGivenIsNullWithCustomMessage()
		{
			Require.IsNotNull<object>("testValue", null, "is garbage");
			Assert.Fail("should have blown up on null value");
		}

		[TestMethod]
		public void Ensure_IsNotNullPassesWhenValueGivenIsNotNull()
		{
			object o = new Object();
			Require.IsNotNull<object>(o);
		}

		[TestMethod]
		public void Ensure_IsNotEmptyPassesWhenEnumerableValueGivenIsNotEmpty()
		{
			Collection<object> e = new Collection<Object>();
			e.Add(new Object());
			Require.IsNotEmpty<object>(e);
		}

		[TestMethod]
		[ExpectedException(typeof(PreconditionContractException))]
		public void Ensure_IsNotEmptyFailsWhenEnumerableValueGivenIsEmpty()
		{
			Collection<object> e = new Collection<Object>();
			Require.IsNotEmpty<object>(e);
		}

		[TestMethod]
		public void Ensure_IsNotEmptyPassesWhenStringValueGivenIsNotEmpty()
		{
			String s = "not empty";
			Require.IsNotEmpty(s);
		}

		[TestMethod]
		[ExpectedException(typeof(PreconditionContractException))]
		public void Ensure_IsNotEmptyFailsWhenStringValueGivenIsEmpty()
		{
			String s = String.Empty;
			Require.IsNotEmpty(s);
		}

		[TestMethod]
		public void Ensure_IsInstanceOfTypePassesWhenIsInstance()
		{
			String s = String.Empty;
			Require.IsInstanceOfType<string>(s);
		}

		[TestMethod]
		[ExpectedException(typeof(PreconditionContractException))]
		public void Ensure_IsInstanceOfTypeFailsWhenIsNotInstance()
		{
			String s = String.Empty;
			Require.IsInstanceOfType<Type>(s);
		}

		[TestMethod]
		public void Ensure_IsGreaterThanPassesWhenComparableIsGreater()
		{
			var values = new { Comparand = 1, Value = 0 };
			Require.IsGreaterThan(values.Comparand, values.Value);
		}

		[TestMethod]
		public void Ensure_IsGreaterThanPassesWhenComparableIsGreater_Strings()
		{
			var values = new { Comparand = "Zzxxyy", Value = "AAbbcc" };
			Require.IsGreaterThan(values.Comparand, values.Value);
		}

		[TestMethod]
		[ExpectedException(typeof(PreconditionContractException))]
		public void Ensure_IsGreaterThanFailsWhenComparableIsLessThan()
		{
			var values = new { Comparand = 0, Value = 1 };
			Require.IsGreaterThan(values.Comparand, values.Value);
		}

		[TestMethod]
		[ExpectedException(typeof(PreconditionContractException))]
		public void Ensure_IsGreaterThanFailsWhenComparableIsLessThan_Strings()
		{
			var values = new { Comparand = "AAbbcc", Value = "Zzxxyy" };
			Require.IsGreaterThan(values.Comparand, values.Value);
		}

		[TestMethod]
		[ExpectedException(typeof(PreconditionContractException))]
		public void Ensure_IsGreaterThanFailsWhenComparableIsEqual()
		{
			var values = new { Comparand = 45, Value = 45 };
			Require.IsGreaterThan(values.Comparand, values.Value);
		}

		[TestMethod]
		[ExpectedException(typeof(PreconditionContractException))]
		public void Ensure_IsGreaterThanFailsWhenComparableIsEqual_Strings()
		{
			var values = new { Comparand = "Value is equal", Value = "Value is equal" };
			Require.IsGreaterThan(values.Comparand, values.Value);
		}

		[TestMethod]
		[ExpectedException(typeof(InvariantContractException))]
		public void InvariantContracts_CalledDirectly()
		{
			InvariantContracts();
			Assert.Fail();
		}

		string _invariantTestVariable = null;
		[InvariantContract]
		private void InvariantContracts()
		{
			Contracts.Invariant.IsNotNull("_invariantTestVariable", _invariantTestVariable);
		}

		[TestMethod]
		[ExpectedException(typeof(InvariantContractException))]
		public void InvariantContracts_CalledInDirectly()
		{
			Contracts.Invariant.Enforce(this);
			Assert.Fail();
		}

		[TestMethod]
		public void CollectionCallWithIterator()
		{
			Stopwatch t = new Stopwatch();
			Collection<string> names = new Collection<string>();
			for (int i = 0; i < 1000000; i++)
			{
				names.Add(String.Concat("This is Name #", i.ToString("X")));
			}
			t.Start();
			CountCollection(names);
			t.Stop();
			Console.WriteLine(t.Elapsed);
		}

		private void CountCollection(ICollection<string> items)
		{
			int i = 0;
			foreach (string item in items)
			{
				i++;
			}
		}

		[TestMethod]
		public void CollectionCallByListCast()
		{
			Stopwatch t = new Stopwatch();
			List<string> names = new List<string>();
			for (int i = 0; i < 1000000; i++)
			{
				names.Add(String.Concat("This is Name #", i.ToString("X")));
			}
			t.Start();
			CountList(names);
			t.Stop();
			Console.WriteLine(t.Elapsed);
		}

		private void CountList(ICollection<string> items)
		{
			int j = 0;
			if (items is IList<string>)
			{
				for (int i = 0; i < items.Count; i++)
				{
					j++;
				}
			}
			else
			{
				foreach (string item in items)
				{
					j++;
				}
			}
		}
	}

}
