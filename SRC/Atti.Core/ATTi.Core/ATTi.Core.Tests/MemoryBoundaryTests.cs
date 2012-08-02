using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ATTi.Core.Tests.Memento;
using ATTi.Core.Mementos.Helpers;

namespace ATTi.Core.Tests
{
	[TestClass]
	public class MemoryBoundaryTests
	{
		[TestMethod]
		public void MemoryBoundary_CreateAndCommitEmpty()
		{			
			using (MemoryBoundary mem = new MemoryBoundary())
			{
				mem.MarkComplete();
			}
		}

		[TestMethod]
		public void MemoryBoundary_CreateAndCommit_NoGraph()
		{
			var test = new
				{
					Obj = new Obj(1, "the obj", 2.3d),
					Sub = new Sub(2, "the sub.base", 6.4d, 3, "the sub", null),
					Sub2 = new Sub(4, "the sub.base 2", 9.4d, 5, "the sub 2", null),
				};

			Obj obj = test.Obj.Clone();

			using (MemoryBoundary mem = new MemoryBoundary())
			{
				mem.Capture(obj);

				obj.F = test.Sub2.F;
				obj.F1 = test.Sub2.F1;
				obj.F2 = test.Sub2.F2;

				mem.MarkComplete();
			}

			Assert.AreEqual(test.Sub2.F, obj.F);
			Assert.AreEqual(test.Sub2.F1, obj.F1);
			Assert.AreEqual(test.Sub2.F2, obj.F2);
		}

		[TestMethod]
		public void MemoryBoundary_CaptureAndRestore_NoGraph()
		{
			var test = new
			{
				Obj = new Obj(1, "the obj", 2.3d),
				Sub = new Sub(2, "the sub.base", 6.4d, 3, "the sub", null),
				Sub2 = new Sub(4, "the sub.base", 9.4d, 5, "the sub", null),
			};

			Obj obj = test.Obj.Clone();

			using (MemoryBoundary mem = new MemoryBoundary())
			{
				mem.Capture(obj);

				obj.F = test.Sub2.F;
				obj.F1 = test.Sub2.F1;
				obj.F2 = test.Sub2.F2;

				// Assert the object changed...
				Assert.AreEqual(test.Sub2.F, obj.F);
				Assert.AreEqual(test.Sub2.F1, obj.F1);
				Assert.AreEqual(test.Sub2.F2, obj.F2);
				Assert.AreNotEqual(test.Obj.F, obj.F);
				Assert.AreNotEqual(test.Obj.F1, obj.F1);
				Assert.AreNotEqual(test.Obj.F2, obj.F2);

				// Not marking complete, this will cause
				// a rollback of all captured objects...
			}

			// Assert the objects were rolled back...
			Assert.AreEqual(test.Obj.F, obj.F);
			Assert.AreEqual(test.Obj.F1, obj.F1);
			Assert.AreEqual(test.Obj.F2, obj.F2);
		}

		[TestMethod]
		public void MemoryBoundary_CreateAndCommit_WithShallowGraph()
		{
			var test = new
			{
				Obj = new Obj(1, "the obj", 2.3d),
				Sub = new Sub(2, "the sub.base", 6.4d, 3, "the sub", null),
				Sub2 = new Sub(4, "the sub2.base", 9.4d, 5, "the sub2", null),
			};

			Obj obj = test.Obj.Clone();
			Sub sub = (Sub)test.Sub.Clone();
			Sub sub2 = (Sub)test.Sub2.Clone();
			sub2.Owner = test.Obj;

			using (MemoryBoundary mem = new MemoryBoundary())
			{
				mem.Capture(sub2);

				sub2.F = sub.F;
				sub2.F1 = sub.F1;
				sub2.F2 = sub.F2;
				sub2.S = sub.S;
				sub2.S1 = sub.S1;

				// Assert the object changed...
				Assert.AreEqual(test.Sub.F, sub2.F);
				Assert.AreEqual(test.Sub.F1, sub2.F1);
				Assert.AreEqual(test.Sub.F2, sub2.F2);
				Assert.AreEqual(test.Sub.S, sub2.S);
				Assert.AreEqual(test.Sub.S1, sub2.S1);
				Assert.AreNotEqual(test.Sub.Owner, sub2.Owner);

				mem.MarkComplete();
			}

			// Assert the change survived the memory boundary.
			Assert.AreEqual(test.Sub.F, sub2.F);
			Assert.AreEqual(test.Sub.F1, sub2.F1);
			Assert.AreEqual(test.Sub.F2, sub2.F2);
			Assert.AreEqual(test.Sub.S, sub2.S);
			Assert.AreEqual(test.Sub.S1, sub2.S1);
			Assert.AreNotEqual(test.Sub.Owner, sub2.Owner);
		}

		[TestMethod]
		public void MemoryBoundary_CaptureAndRestore_WithShallowGraph()
		{
			var test = new
			{
				Obj = new Obj(1, "the obj", 2.3d),
				Sub = new Sub(2, "the sub.base", 6.4d, 3, "the sub", null),
				Sub2 = new Sub(4, "the sub2.base", 9.4d, 5, "the sub2", null),
			};

			Obj obj = test.Obj.Clone();
			Sub sub = (Sub)test.Sub.Clone();
			Sub sub2 = (Sub)test.Sub2.Clone();
			sub2.Owner = test.Obj;

			using (MemoryBoundary mem = new MemoryBoundary())
			{
				mem.Capture(sub2);

				sub2.F = sub.F;
				sub2.F1 = sub.F1;
				sub2.F2 = sub.F2;
				sub2.S = sub.S;
				sub2.S1 = sub.S1;

				Assert.AreEqual(test.Sub.F, sub2.F);
				Assert.AreEqual(test.Sub.F1, sub2.F1);
				Assert.AreEqual(test.Sub.F2, sub2.F2);
				Assert.AreEqual(test.Sub.S, sub2.S);
				Assert.AreEqual(test.Sub.S1, sub2.S1);
				Assert.AreNotEqual(test.Sub.Owner, sub2.Owner);
				Assert.AreEqual(test.Obj, sub2.Owner);
			}

			Assert.AreNotEqual(test.Sub.F, sub2.F);
			Assert.AreNotEqual(test.Sub.F1, sub2.F1);
			Assert.AreNotEqual(test.Sub.F2, sub2.F2);
			Assert.AreNotEqual(test.Sub.S, sub2.S);
			Assert.AreNotEqual(test.Sub.S1, sub2.S1);
			Assert.AreNotEqual(test.Sub.Owner, sub2.Owner);
			Assert.AreEqual(test.Obj, sub2.Owner);
		}

		[TestMethod]
		public void MemoryBoundary_CaptureAndRestore_WithShallowRecursiveGraph()
		{
			var test = new
			{
				Obj = new Obj(1, "the obj", 2.3d),
				Sub = new Sub(2, "the sub.base", 6.4d, 3, "the sub", null),
				Sub2 = new Sub(4, "the sub2.base", 9.4d, 5, "the sub2", null),
			};

			Obj obj = test.Obj.Clone();
			Sub sub = (Sub)test.Sub.Clone();
			Sub sub2 = (Sub)test.Sub2.Clone();
			sub.Owner = obj;
			sub2.Owner = sub;

			using (MemoryBoundary mem = new MemoryBoundary())
			{
				mem.Capture(sub2);

				sub2.F = sub.F;
				sub2.F1 = sub.F1;
				sub2.F2 = sub.F2;
				sub2.S = sub.S;
				sub2.S1 = sub.S1;
				sub2.Owner = obj;

				Assert.AreEqual(test.Sub.F, sub2.F);
				Assert.AreEqual(test.Sub.F1, sub2.F1);
				Assert.AreEqual(test.Sub.F2, sub2.F2);
				Assert.AreEqual(test.Sub.S, sub2.S);
				Assert.AreEqual(test.Sub.S1, sub2.S1);
				Assert.AreNotEqual(test.Sub.Owner, sub2.Owner);
				Assert.AreEqual(obj, sub2.Owner);
			}

			Assert.AreNotEqual(test.Sub.F, sub2.F);
			Assert.AreNotEqual(test.Sub.F1, sub2.F1);
			Assert.AreNotEqual(test.Sub.F2, sub2.F2);
			Assert.AreNotEqual(test.Sub.S, sub2.S);
			Assert.AreNotEqual(test.Sub.S1, sub2.S1);
			Assert.AreNotEqual(test.Sub.Owner, sub2.Owner);
			Assert.AreSame(sub, sub2.Owner);
		}

		[TestMethod]
		public void MemoryBoundary_CaptureAndRestore_WithShallowCyclicGraph()
		{
			var test = new
			{
				Obj = new Obj(1, "the obj", 2.3d),
				Sub = new Sub(2, "the sub.base", 6.4d, 3, "the sub", null),
				Sub2 = new Sub(4, "the sub2.base", 9.4d, 5, "the sub2", null),
			};

			Obj obj = test.Obj.Clone();
			Sub sub = (Sub)test.Sub.Clone();
			Sub sub2 = (Sub)test.Sub2.Clone();
			sub.Owner = sub2;
			sub2.Owner = sub;

			using (MemoryBoundary mem = new MemoryBoundary())
			{
				mem.Capture(sub2);

				sub2.F = sub.F;
				sub2.F1 = sub.F1;
				sub2.F2 = sub.F2;
				sub2.S = sub.S;
				sub2.S1 = sub.S1;
				sub2.Owner = obj;

				Assert.AreEqual(test.Sub.F, sub2.F);
				Assert.AreEqual(test.Sub.F1, sub2.F1);
				Assert.AreEqual(test.Sub.F2, sub2.F2);
				Assert.AreEqual(test.Sub.S, sub2.S);
				Assert.AreEqual(test.Sub.S1, sub2.S1);
				Assert.AreNotEqual(test.Sub.Owner, sub2.Owner);
				Assert.AreEqual(obj, sub2.Owner);
			}

			Assert.AreNotEqual(test.Sub.F, sub2.F);
			Assert.AreNotEqual(test.Sub.F1, sub2.F1);
			Assert.AreNotEqual(test.Sub.F2, sub2.F2);
			Assert.AreNotEqual(test.Sub.S, sub2.S);
			Assert.AreNotEqual(test.Sub.S1, sub2.S1);
			Assert.AreNotEqual(test.Sub.Owner, sub2.Owner);
			Assert.AreSame(sub, sub2.Owner);
		}

		[TestMethod]
		public void MemoryBoundary_CaptureAndRestore_WithGraphAndPrimitiveArray()
		{
			var test = new
			{
				Obj = new Obj(1, "the obj", 2.3d),
				Sub = new Sub(2, "the sub.base", 6.4d, 3, "the sub", null),
				SubSub = new SubWithPrimitiveArray(4, "the sub2.base", 9.4d, 5, "the sub2", null, 13),
			};

			Obj obj = test.Obj.Clone();
			Sub sub = (Sub)test.Sub.Clone();
			SubWithPrimitiveArray sub2 = (SubWithPrimitiveArray)test.SubSub.Clone();
			sub.Owner = obj;
			sub2.Owner = sub;
			sub2.AddValue(1);
			sub2.AddValue(2);
			sub2.AddValue(3);

			using (MemoryBoundary mem = new MemoryBoundary())
			{
				mem.Capture(sub2);

				sub2.F = sub.F;
				sub2.F1 = sub.F1;
				sub2.F2 = sub.F2;
				sub2.S = sub.S;
				sub2.S1 = sub.S1;
				sub2.Owner = obj;
				sub2.AddValue(4);
				sub2.AddValue(5);


				Assert.AreEqual(test.Sub.F, sub2.F);
				Assert.AreEqual(test.Sub.F1, sub2.F1);
				Assert.AreEqual(test.Sub.F2, sub2.F2);
				Assert.AreEqual(test.Sub.S, sub2.S);
				Assert.AreEqual(test.Sub.S1, sub2.S1);
				Assert.AreNotEqual(test.Sub.Owner, sub2.Owner);
				Assert.AreEqual(5, sub2.Values.Count);
				Assert.AreEqual(1, sub2.Values[0]);
				Assert.AreEqual(2, sub2.Values[1]);
				Assert.AreEqual(3, sub2.Values[2]);
				Assert.AreEqual(4, sub2.Values[3]);
				Assert.AreEqual(5, sub2.Values[4]);
				Assert.AreEqual(obj, sub2.Owner);
			}

			Assert.AreNotEqual(test.Sub.F, sub2.F);
			Assert.AreNotEqual(test.Sub.F1, sub2.F1);
			Assert.AreNotEqual(test.Sub.F2, sub2.F2);
			Assert.AreNotEqual(test.Sub.S, sub2.S);
			Assert.AreNotEqual(test.Sub.S1, sub2.S1);
			Assert.AreEqual(3, sub2.Values.Count);
			Assert.AreEqual(1, sub2.Values[0]);
			Assert.AreEqual(2, sub2.Values[1]);
			Assert.AreEqual(3, sub2.Values[2]);
			Assert.AreNotEqual(test.Sub.Owner, sub2.Owner);
		}

		[TestMethod]
		public void MemoryBoundary_CaptureAndRestore_WithGraphAndObjectArray()
		{
			var test = new
			{
				Obj = new Obj(1, "the obj", 2.3d),
				Sub = new Sub(2, "the sub.base", 6.4d, 3, "the sub", null),
				SubSub = new SubWithArrayOfObjects(4, "the sub2.base", 9.4d, 5, "the sub2", null, 13),
				o = new Obj(),
				o1 = new Obj(),
				o2 = new Obj(),
				o3 = new Obj(),
				o4 = new Obj(),
				o5 = new Obj(),
			};

			Obj obj = test.Obj.Clone();
			Sub sub = (Sub)test.Sub.Clone();
			SubWithArrayOfObjects sub2 = (SubWithArrayOfObjects)test.SubSub.Clone();
			sub.Owner = obj;
			sub2.Owner = sub;
			sub2.AddValue(test.o1);
			sub2.AddValue(test.o2);
			sub2.AddValue(test.o3);

			using (MemoryBoundary mem = new MemoryBoundary())
			{
				mem.Capture(sub2);

				sub2.F = sub.F;
				sub2.F1 = sub.F1;
				sub2.F2 = sub.F2;
				sub2.S = sub.S;
				sub2.S1 = sub.S1;
				sub2.Owner = obj;
				sub2.AddValue(test.o4);
				sub2.AddValue(test.o5);


				Assert.AreEqual(test.Sub.F, sub2.F);
				Assert.AreEqual(test.Sub.F1, sub2.F1);
				Assert.AreEqual(test.Sub.F2, sub2.F2);
				Assert.AreEqual(test.Sub.S, sub2.S);
				Assert.AreEqual(test.Sub.S1, sub2.S1);
				Assert.AreNotEqual(test.Sub.Owner, sub2.Owner);
				Assert.AreEqual(5, sub2.Values.Count);
				Assert.AreEqual(test.o1, sub2.Values[0]);
				Assert.AreEqual(test.o2, sub2.Values[1]);
				Assert.AreEqual(test.o3, sub2.Values[2]);
				Assert.AreEqual(test.o4, sub2.Values[3]);
				Assert.AreEqual(test.o5, sub2.Values[4]);
				Assert.AreEqual(obj, sub2.Owner);
			}

			Assert.AreNotEqual(test.Sub.F, sub2.F);
			Assert.AreNotEqual(test.Sub.F1, sub2.F1);
			Assert.AreNotEqual(test.Sub.F2, sub2.F2);
			Assert.AreNotEqual(test.Sub.S, sub2.S);
			Assert.AreNotEqual(test.Sub.S1, sub2.S1);
			Assert.AreEqual(3, sub2.Values.Count);
			Assert.AreEqual(test.o1, sub2.Values[0]);
			Assert.AreEqual(test.o2, sub2.Values[1]);
			Assert.AreEqual(test.o3, sub2.Values[2]);
			Assert.AreNotEqual(test.Sub.Owner, sub2.Owner);
		}
				
		[TestMethod]
		public void MemoryBoundary_CaptureAndRestore_WithGraphAndListOfObject()
		{			
			var test = new
			{
				Obj = new Obj(1, "the obj", 2.3d),
				Sub = new Sub(2, "the sub.base", 6.4d, 3, "the sub", null),
				SubSub = new SubWithListField(4, "the sub2.base", 9.4d, 5, "the sub2", null, 13),
				o = new Obj(),
				o1 = new Obj(),
				o2 = new Obj(),
				o3 = new Obj(),
				o4 = new Obj(),
				o5 = new Obj(),
			};

			Obj obj = test.Obj.Clone();
			Sub sub = (Sub)test.Sub.Clone();
			SubWithListField sub2 = (SubWithListField)test.SubSub.Clone();
			sub.Owner = obj;
			sub2.Owner = sub;
			sub2.Values.Add(test.o1);
			sub2.Values.Add(test.o2);
			sub2.Values.Add(test.o3);

			using (MemoryBoundary mem = new MemoryBoundary())
			{
				mem.Capture(sub2);

				sub2.F = sub.F;
				sub2.F1 = sub.F1;
				sub2.F2 = sub.F2;
				sub2.S = sub.S;
				sub2.S1 = sub.S1;
				sub2.Owner = obj;
				sub2.Values.Add(test.o4);
				sub2.Values.Add(test.o5);


				Assert.AreEqual(test.Sub.F, sub2.F);
				Assert.AreEqual(test.Sub.F1, sub2.F1);
				Assert.AreEqual(test.Sub.F2, sub2.F2);
				Assert.AreEqual(test.Sub.S, sub2.S);
				Assert.AreEqual(test.Sub.S1, sub2.S1);
				Assert.AreNotEqual(test.Sub.Owner, sub2.Owner);
				Assert.AreEqual(5, sub2.Values.Count);
				Assert.AreEqual(test.o1, sub2.Values[0]);
				Assert.AreEqual(test.o2, sub2.Values[1]);
				Assert.AreEqual(test.o3, sub2.Values[2]);
				Assert.AreEqual(test.o4, sub2.Values[3]);
				Assert.AreEqual(test.o5, sub2.Values[4]);
				Assert.AreEqual(obj, sub2.Owner);
			}

			Assert.AreNotEqual(test.Sub.F, sub2.F);
			Assert.AreNotEqual(test.Sub.F1, sub2.F1);
			Assert.AreNotEqual(test.Sub.F2, sub2.F2);
			Assert.AreNotEqual(test.Sub.S, sub2.S);
			Assert.AreNotEqual(test.Sub.S1, sub2.S1);
			Assert.AreEqual(3, sub2.Values.Count);
			Assert.AreEqual(test.o1, sub2.Values[0]);
			Assert.AreEqual(test.o2, sub2.Values[1]);
			Assert.AreEqual(test.o3, sub2.Values[2]);
			Assert.AreNotEqual(test.Sub.Owner, sub2.Owner);
		}

		[TestMethod]
		public void MemoryBoundary_CaptureAndRestore_Dictionary()
		{
			var test = new
			{
				Item1 = new ATTi.Core.Collections.KeyValuePair<string, Obj>("Item 1", new Obj(1, "1.1", 1.1)),
				Item2 = new ATTi.Core.Collections.KeyValuePair<string, Obj>("Item 2", new Obj(2, "2.2", 2.2)),
				Item3 = new ATTi.Core.Collections.KeyValuePair<string, Obj>("Item 3", new Obj(3, "3.3", 3.3)),
				Item4 = new ATTi.Core.Collections.KeyValuePair<string, Obj>("Item 1", new Obj(4, "4.4", 4.4)),
				Item5 = new ATTi.Core.Collections.KeyValuePair<string, Obj>("Item 2", new Obj(5, "5.5", 5.5)),
				Item6 = new ATTi.Core.Collections.KeyValuePair<string, Obj>("Item 3", new Obj(6, "6.6", 6.6)),
			};

			Dictionary<string, Obj> items = new Dictionary<string, Obj>();
			items.Add(test.Item1.Key, test.Item1.Value);
			items.Add(test.Item2.Key, test.Item2.Value);
			items.Add(test.Item3.Key, test.Item3.Value);

			using (MemoryBoundary mem = new MemoryBoundary())
			{
				mem.Capture(items);

				Assert.AreEqual(items[test.Item1.Key], test.Item1.Value);
				Assert.AreEqual(items[test.Item2.Key], test.Item2.Value);
				Assert.AreEqual(items[test.Item3.Key], test.Item3.Value);

				items[test.Item4.Key] = test.Item4.Value;
				items[test.Item5.Key] = test.Item5.Value;
				items[test.Item6.Key] = test.Item6.Value;

				Assert.AreNotEqual(items[test.Item1.Key], test.Item1.Value);
				Assert.AreNotEqual(items[test.Item2.Key], test.Item2.Value);
				Assert.AreNotEqual(items[test.Item3.Key], test.Item3.Value);

				Assert.IsTrue(items.Count == 3);
				// Not committed so the dictionary's content will be rolled back.
			}

			Assert.IsTrue(items.Count == 3);

			Assert.AreEqual(items[test.Item1.Key], test.Item1.Value);
			Assert.AreEqual(items[test.Item2.Key], test.Item2.Value);
			Assert.AreEqual(items[test.Item3.Key], test.Item3.Value);
		}
	}
}
