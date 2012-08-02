using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ATTi.Core.Tests.Factory.Classes;
using ATTi.Core.Factory;

namespace ATTi.Core.Tests.Factory
{
	/// <summary>
	/// Tests the Factory
	/// </summary>
	[TestClass]
	public class FactoryTests
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void Factory_ClassWithDefaultConstructor_NoInstanceReusePolicy()
		{
			var test = new
			{
				NumberOfInstances = 1000
			};

			Factory<ClassWithDefaultConstructor>.Wireup.SetInstanceReusePolicy(InstanceReusePolicy.None);

			int priorID = 0;
			for (int i = 0; i < test.NumberOfInstances; i++)
			{
				ClassWithDefaultConstructor instance = Factory<ClassWithDefaultConstructor>.CreateInstance();
				Assert.IsNotNull(instance);
				Assert.AreNotEqual(priorID, instance.ID);
				priorID = instance.ID;
			}
		}

		[TestMethod]
		public void Factory_ClassWithDefaultConstructor_NoInstanceReusePolicy_MultiThreaded()
		{
			var test = new
			{
				NumberOfBackgroundJobs = 50,
				NumberOfInstances = 1000
			};
			Factory<ClassWithDefaultConstructor>.Wireup.SetInstanceReusePolicy(InstanceReusePolicy.None);
			ManualResetEvent beginRace = new ManualResetEvent(false);

			ClassWithDefaultConstructor_ThreadState[] threads = new ClassWithDefaultConstructor_ThreadState[test.NumberOfBackgroundJobs];
			for (int i = 0; i < threads.Length; i++)
			{
				threads[i] = new ClassWithDefaultConstructor_ThreadState
				{
					NumberOfInstances = test.NumberOfInstances,
					Event = new ManualResetEvent(false),
					BeginRace = beginRace,
					Error = null
				};
			}
			foreach (ClassWithDefaultConstructor_ThreadState t in threads)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(Background_Factory_ClassWithDefaultConstructor_NoInstanceReusePolicy), t);
			}
			// Give background threads a chance to wait on the beginRace event...
			Thread.Sleep(500);
			beginRace.Set();
			foreach (ClassWithDefaultConstructor_ThreadState t in threads)
			{
				t.Event.WaitOne();
				if (t.Error != null) throw t.Error;
			}
		}

		private void Background_Factory_ClassWithDefaultConstructor_NoInstanceReusePolicy(object state)
		{
			ClassWithDefaultConstructor_ThreadState test = (ClassWithDefaultConstructor_ThreadState)state;
			try
			{
				int priorID = 0;
				test.BeginRace.WaitOne();
				for (int i = 0; i < test.NumberOfInstances; i++)
				{
					ClassWithDefaultConstructor instance = Factory<ClassWithDefaultConstructor>.CreateInstance();
					Assert.IsNotNull(instance);
					Assert.AreNotEqual(priorID, instance.ID);
					priorID = instance.ID;
				}
			}
			catch (Exception e)
			{
				test.Error = e;
			}
			finally
			{
				test.Event.Set();
			}
		}
		class ClassWithDefaultConstructor_ThreadState
		{
			public int NumberOfInstances;
			public ManualResetEvent Event;
			public ManualResetEvent BeginRace;
			public Exception Error;
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException), "No concrete type registered for Test.ATTi.Core.Factory.Classes.INoDefaultImplementation.")]
		public void Factory_CreateInstanceFromInterface_InvalidOperationException_NoConcreteType()
		{
			INoDefaultImplementation instance = Factory<INoDefaultImplementation>.CreateInstance();
			Assert.Fail("Should have thrown a InvalidOperationException");
		}

		[TestMethod]
		public void Factory_CreateInstanceFromInterface_WithDefaultImplementation()
		{
			var test = new
			{
				NumberOfInstances = 1000
			};

			Factory<IClassWithID>.Wireup.SetDefaultImplementation<ClassWithDefaultConstructor>();
			int priorID = 0;
			for (int i = 0; i < test.NumberOfInstances; i++)
			{
				IClassWithID instance = Factory<IClassWithID>.CreateInstance();
				Assert.IsNotNull(instance);
				Assert.AreNotEqual(priorID, instance.ID);
				priorID = instance.ID;
			}
		}

		[TestMethod]
		public void Factory_NotificationOrderingTest()
		{
			Factory<ClassWithDefaultConstructor>.InstanceAction += new FactoryInstanceEvent<ClassWithDefaultConstructor>(ClassWithDefaultConstructor_InstanceAction);
			Factory<IClassWithID>.InstanceAction += new FactoryInstanceEvent<IClassWithID>(IClassWithID_InstanceAction);
			Factory<Object>.InstanceAction += new FactoryInstanceEvent<object>(Object_InstanceAction);

			Factory<IClassWithID>.Wireup.SetDefaultImplementation<ClassWithDefaultConstructor>();

			IClassWithID instance = Factory<IClassWithID>.CreateInstance();
			Assert.IsNotNull(instance);
			Assert.AreEqual(3, _factory_NotificationOrderingTest_typeList.Count());
			Assert.AreEqual(typeof(ClassWithDefaultConstructor), _factory_NotificationOrderingTest_typeList.Pop());
			Assert.AreEqual(typeof(Object), _factory_NotificationOrderingTest_typeList.Pop());
			Assert.AreEqual(typeof(IClassWithID), _factory_NotificationOrderingTest_typeList.Pop());

			Factory<IClassWithID>.InstanceAction -= new FactoryInstanceEvent<IClassWithID>(IClassWithID_InstanceAction);
			Factory<Object>.InstanceAction -= new FactoryInstanceEvent<object>(Object_InstanceAction);
		}

		void ClassWithDefaultConstructor_InstanceAction(Type requestedType, ClassWithDefaultConstructor instance, string name, FactoryAction action)
		{
			if (_factory_NotificationOrderingTest_typeList != null)
			{
				lock (_factory_NotificationOrderingTest_typeList)
					_factory_NotificationOrderingTest_typeList.Push(typeof(ClassWithDefaultConstructor));
			}
		}
		void IClassWithID_InstanceAction(Type requestedType, IClassWithID instance, string name, FactoryAction action)
		{
			if (_factory_NotificationOrderingTest_typeList != null)
			{
				lock (_factory_NotificationOrderingTest_typeList)
					_factory_NotificationOrderingTest_typeList.Push(typeof(IClassWithID));
			}
		}
		Stack<Type> _factory_NotificationOrderingTest_typeList = new Stack<Type>();
		void Object_InstanceAction(Type requestedType, object instance, string name, FactoryAction action)
		{
			if (_factory_NotificationOrderingTest_typeList != null)
			{
				lock (_factory_NotificationOrderingTest_typeList)
					_factory_NotificationOrderingTest_typeList.Push(typeof(Object));
			}
		}
	}
}
