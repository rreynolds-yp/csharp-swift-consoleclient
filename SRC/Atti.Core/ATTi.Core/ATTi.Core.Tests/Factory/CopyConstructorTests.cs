using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ATTi.Core;
using ATTi.Core.Factory;
using ATTi.Core.Contracts;
using System.Xml.Linq;

namespace ATTi.Core.Tests.Factory
{
	[TestClass]
	public class Factory_CopyConstructorTests
	{
		public class Item
		{
			public string Value { get; set; }
		}

		public class Item1
		{
			public string Value { get; set; }
		}

		public class Item2
		{
			public string Value { get; set; }
		}

		public class Item3
		{
			public string Value { get; set; }
			public int Index { get; set; }
		}

		public class ItemSubclass : Item2
		{
			public int Index { get; set; }
		}

		public class Item4
		{
			public string Value { get; set; }
			public int Index { get; set; }
			public Item3 Item3 { get; set; }
		}

		[TestMethod]
		public void Factory_Copier_CopyStrict()
		{
			var data = new
			{
				Item1 = new { Value = "Item 1" },
				Item2 = new { Value = "Item 2" },
				Item3 = new { Value = "Item 3" },
				Item4 = new { Value = "Item 4" },
				Item5 = new { Value = "Item 5" }
			};

			var source = data.Item1;
			Item item = Factory<Item>.Copier<Item>.CopyStrict(source);
			Assert.IsNotNull(item);
			Assert.AreNotSame(item, source);
			Assert.AreEqual(item.Value, source.Value);

			source = data.Item2;
			item = Factory<Item>.Copier<Item>.CopyStrict(source);
			Assert.IsNotNull(item);
			Assert.AreNotSame(item, source);
			Assert.AreEqual(item.Value, source.Value);

			source = data.Item3;
			item = Factory<Item>.Copier<Item>.CopyStrict(source);
			Assert.IsNotNull(item);
			Assert.AreNotSame(item, source);
			Assert.AreEqual(item.Value, source.Value);

			source = data.Item4;
			item = Factory<Item>.Copier<Item>.CopyStrict(source);
			Assert.IsNotNull(item);
			Assert.AreNotSame(item, source);
			Assert.AreEqual(item.Value, source.Value);

			source = data.Item5;
			item = Factory<Item>.Copier<Item>.CopyStrict(source);
			Assert.IsNotNull(item);
			Assert.AreNotSame(item, source);
			Assert.AreEqual(item.Value, source.Value);
		}

		[TestMethod]
		[ExpectedException(typeof(InvariantContractException))]
		public void Factory_Copier_CopyStrict_TargetMissingProperty()
		{
			var data = new
			{
				Item1 = new { Value = "Item 1", Index = 1 },
				Item2 = new { Value = "Item 2", Index = 2 },
				Item3 = new { Value = "Item 3", Index = 3 },
				Item4 = new { Value = "Item 4", Index = 4 },
				Item5 = new { Value = "Item 5", Index = 5 }
			};

			var source = data.Item1;
			Item1 item = Factory<Item1>.Copier<Item1>.CopyStrict(source);
			Assert.Fail(@"TypeAssertionException should have been thrown because the target (Item1) 
does not have a target property for the anonymous type's Index property."
				);
		}

		[TestMethod]
		public void Factory_Copier_CopyLoose()
		{
			var data = new
			{
				Item1 = new { Value = "Item 1", Index = 1 },
				Item2 = new { Value = "Item 2", Index = 2 },
				Item3 = new { Value = "Item 3", Index = 3 },
				Item4 = new { Value = "Item 4", Index = 4 },
				Item5 = new { Value = "Item 5", Index = 5 }
			};

			var source = data.Item1;
			Item2 item = Factory<Item2>.Copier<Item2>.CopyLoose(source);
			Assert.IsNotNull(item);
			Assert.AreNotSame(item, source);
			Assert.AreEqual(item.Value, source.Value);

			source = data.Item2;
			item = Factory<Item2>.Copier<Item2>.CopyLoose(source);
			Assert.IsNotNull(item);
			Assert.AreNotSame(item, source);
			Assert.AreEqual(item.Value, source.Value);

			source = data.Item3;
			item = Factory<Item2>.Copier<Item2>.CopyLoose(source);
			Assert.IsNotNull(item);
			Assert.AreNotSame(item, source);
			Assert.AreEqual(item.Value, source.Value);

			source = data.Item4;
			item = Factory<Item2>.Copier<Item2>.CopyLoose(source);
			Assert.IsNotNull(item);
			Assert.AreNotSame(item, source);
			Assert.AreEqual(item.Value, source.Value);

			source = data.Item5;
			item = Factory<Item2>.Copier<Item2>.CopyLoose(source);
			Assert.IsNotNull(item);
			Assert.AreNotSame(item, source);
			Assert.AreEqual(item.Value, source.Value);
		}

		[TestMethod]
		public void Factory_Copier_CopyLoose_WithCascade()
		{
			var data = new
			{
				Item4 = new { Value = "Item4 1", Index = 1, Item3 = new { Value = "Item3 1", Index = 13 } }
			};

			Item4 copy = Factory<Item4>.Copier<Item4>.CopyLoose(data.Item4);
			Assert.IsNotNull(copy);
			Assert.AreEqual(data.Item4.Value, copy.Value);
			Assert.AreEqual(data.Item4.Index, copy.Index);
			Assert.IsNotNull(copy.Item3);
			Assert.AreEqual(data.Item4.Item3.Value, copy.Item3.Value);
			Assert.AreEqual(data.Item4.Item3.Index, copy.Item3.Index);
		}

		[TestMethod]
		public void Factory_Copier_CopyLoose_WithCascadeAndHandleNull()
		{
			var data = new
			{
				Item4 = new { Value = "Item4 1", Index = 1, Item3 = default(Item2) }
			};

			Item4 copy = Factory<Item4>.Copier<Item4>.CopyLoose(data.Item4);
			Assert.IsNotNull(copy);
			Assert.AreEqual(data.Item4.Value, copy.Value);
			Assert.AreEqual(data.Item4.Index, copy.Index);
			Assert.IsNull(copy.Item3);
		}

		[TestMethod]
		public void Factory_Copier_CopyLoose_ItemSubclass()
		{
			var data = new
			{
				Item1 = new { Value = "Item2 1", Index = 1 },
				Item2 = new { Value = "Item2 2", Index = 2 },
				Item3 = new { Value = "Item2 3", Index = 3 },
				Item4 = new { Value = "Item2 4", Index = 4 },
				Item5 = new { Value = "Item2 5", Index = 5 }
			};

			// Base class notifications must occur before subclass notifications
			Factory<Item2>.DefaultFactory.InstanceAction += Factory_CopierorTests_InstanceAction_TrackCallOrdering;
			Factory<ItemSubclass>.InstanceAction += Factory_CopierorTests_InstanceAction_ItemSubclass_TrackCallOrdering;
			try
			{
				var source = data.Item1;
				Item2 item = Factory<Item2>.Copier<ItemSubclass>.CopyLoose(source);
				Assert.IsNotNull(item);
				Assert.AreNotSame(item, source);
				Assert.AreEqual(source.Value, item.Value);
				Assert.AreEqual(source.Index, (item as ItemSubclass).Index);

				source = data.Item2;
				item = Factory<Item2>.Copier<ItemSubclass>.CopyLoose(source);
				Assert.IsNotNull(item);
				Assert.AreNotSame(item, source);
				Assert.AreEqual(item.Value, source.Value);
				Assert.AreEqual(source.Index, (item as ItemSubclass).Index);

				source = data.Item3;
				item = Factory<Item2>.Copier<ItemSubclass>.CopyLoose(source);
				Assert.IsNotNull(item);
				Assert.AreNotSame(item, source);
				Assert.AreEqual(source.Value, item.Value);
				Assert.AreEqual(source.Index, (item as ItemSubclass).Index);

				source = data.Item4;
				item = Factory<Item2>.Copier<ItemSubclass>.CopyLoose(source);
				Assert.IsNotNull(item);
				Assert.AreNotSame(item, source);
				Assert.AreEqual(source.Value, item.Value);
				Assert.AreEqual(source.Index, (item as ItemSubclass).Index);

				source = data.Item5;
				item = Factory<Item2>.Copier<ItemSubclass>.CopyLoose(source);
				Assert.IsNotNull(item);
				Assert.AreNotSame(item, source);
				Assert.AreEqual(source.Value, item.Value);
				Assert.AreEqual(source.Index, (item as ItemSubclass).Index);
			}
			finally
			{
				Factory<Item2>.DefaultFactory.InstanceAction -= Factory_CopierorTests_InstanceAction_TrackCallOrdering;
				Factory<ItemSubclass>.InstanceAction -= Factory_CopierorTests_InstanceAction_ItemSubclass_TrackCallOrdering;
			}
		}

		Type _tracking;
		void Factory_CopierorTests_InstanceAction_TrackCallOrdering(Type requestedType, Factory_CopyConstructorTests.Item2 instance, string name, FactoryAction action)
		{
			Assert.IsNull(Interlocked.CompareExchange<Type>(ref _tracking, typeof(Item), null));
		}
		void Factory_CopierorTests_InstanceAction_ItemSubclass_TrackCallOrdering(Type requestedType, Factory_CopyConstructorTests.ItemSubclass instance, string name, FactoryAction action)
		{
			Assert.AreEqual(Interlocked.CompareExchange<Type>(ref _tracking, null, typeof(Item)), typeof(Item));
		}

		[TestMethod]
		public void Factory_Copier_CopyFromXml()
		{
			var data = new
			{
				Item1 = new { Value = "Item2 1", Index = 1 },
				Item2 = new { Value = "Item2 2", Index = 2 },
				Item3 = new { Value = "Item2 3", Index = 3 },
				Item4 = new { Value = "Item2 4", Index = 4 },
				Item5 = new { Value = "Item2 5", Index = 5 }
			};

			XElement item =
				new XElement("Item3",
					new XElement("Value", data.Item1.Value),
					new XElement("Index", data.Item1.Index));

			Item3 copy = Factory<Item3>.Copier<Item3>.CopyFromXml(item);
			Assert.IsNotNull(copy);
			Assert.AreEqual(data.Item1.Value, copy.Value);
			Assert.AreEqual(data.Item1.Index, copy.Index);
		}

		[TestMethod]
		public void Factory_Copier_CopyFromXml_WithCascade()
		{
			var data = new
			{
				Item4 = new { Value = "Item4 1", Index = 1, Item3 = new { Value = "Item3 1", Index = 13 } }
			};

			XElement item =
				new XElement("Item4",
					new XElement("Value", data.Item4.Value),
					new XElement("Index", data.Item4.Index),
					new XElement("Item3",
						new XElement("Value", data.Item4.Item3.Value),
						new XElement("Index", data.Item4.Item3.Index))
					);

			Item4 copy = Factory<Item4>.Copier<Item4>.CopyFromXml(item);
			Assert.IsNotNull(copy);
			Assert.AreEqual(data.Item4.Value, copy.Value);
			Assert.AreEqual(data.Item4.Index, copy.Index);
			Assert.IsNotNull(copy.Item3);
			Assert.AreEqual(data.Item4.Item3.Value, copy.Item3.Value);
			Assert.AreEqual(data.Item4.Item3.Index, copy.Item3.Index);
		}
	}
}
