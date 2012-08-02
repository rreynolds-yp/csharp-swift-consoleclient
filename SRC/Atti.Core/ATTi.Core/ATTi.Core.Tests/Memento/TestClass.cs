using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATTi.Core.Mementos;

namespace ATTi.Core.Tests.Memento
{
	public class Obj : ICloneable
	{
		struct ObjMemento : IMemento
		{
			private object _target;
			private bool _restored;
			public int Obj_f;
			public string Obj_f1;
			public double Obj_f2;

			public object Target { get { return _target; } set { _target = value; } }
			public bool IsRestored { get { return _restored; } set { _restored = value; } }
		}

		int _f;
		string _f1;
		double _f2;

		public Obj() { }
		public Obj(int f, string f1, double f2)
		{
			_f = f;
			_f1 = f1;
			_f2 = f2;
		}

		public int F { get { return _f; } set { _f = value; } }
		public string F1 { get { return _f1; } set { _f1 = value; } }
		public double F2 { get { return _f2; } set { _f2 = value; } }

		[MementoCapture]
		static IMemento MementoCapture(IMementoContext ctx, Obj item)
		{
			ObjMemento m = new ObjMemento();
			m.Target = item;
			m.Obj_f = item._f;
			m.Obj_f1 = item._f1;
			m.Obj_f2 = item._f2;
			return m;
		}

		[MementoRestore]
		static void MementoRestore(IMementoContext ctx, ref Obj item, object m)
		{
			ObjMemento data = (ObjMemento)m;
			item._f = data.Obj_f;
			item._f1 = data.Obj_f1;
			item._f2 = data.Obj_f2;
		}

		#region ICloneable Members

		public Obj Clone()
		{
			return (Obj)base.MemberwiseClone();
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		#endregion
	}

	

	public class Sub : Obj
	{
		int _s;
		string _s1;
		Obj _owner;

		public Sub() {}
		public Sub(int f, string f1, double f2, int s, string s1, Obj owner)
			: base(f, f1, f2)
		{
			_s = s;
			_s1 = s1;
			_owner = owner;
		}

		public int S { get { return _s; } set { _s = value; } }
		public string S1 { get { return _s1; } set { _s1 = value; } }
		public Obj Owner { get { return _owner; } set { _owner = value; } }

		//[MementoCapture]
		//static IMemento MementoCapture(IMementoContext ctx, Sub item)
		//{
		//  SubMemento m;
		//  m.BaseMemento = ATTi.Core.Mementos.Memento.CaptureBaseMemento<Obj>(ctx, item);
		//  m.Sub_s = item._s;
		//  m.Sub_s1 = item._s1;
		//  m.Sub_owner = ATTi.Core.Mementos.Memento.CaptureMemento<Obj>(ctx, item._owner);
		//  return m;
		//}

		//[MementoRestore]
		//static void MementoRestore(IMementoContext ctx, ref Sub item, object m)
		//{
		//  SubMemento data = (SubMemento)m;			
		//  Obj b = item;
		//  ATTi.Core.Mementos.Memento.RestoreBaseMemento<Obj>(ctx, ref b, data.BaseMemento);
		//  item._s = data.Sub_s;
		//  item._s1 = data.Sub_s1;
		//  ATTi.Core.Mementos.Memento.RestoreMemento(ctx, ref item._owner, data.Sub_owner);
		//}
	}

	public struct SubMemento : IMemento
	{
		public IMemento BaseMemento;
		public int Sub_s;
		public string Sub_s1;
		public IMemento Sub_owner;

		#region IMemento Members

		public object Target { get { return BaseMemento.Target; }	}
		public bool IsRestored
		{
			get { return BaseMemento.IsRestored; }
			set { BaseMemento.IsRestored = value; }
		}

		#endregion
	}

	public class SubWithPrimitiveArray : Sub
	{
		static Obj Blibber = new Obj();
		int _ss;
		int[] _values = new int[0];

		public struct SubSubMemento : IMemento
		{
			public IMemento BaseMemento;
			public IMemento SubSub_Blibber;
			public int SubSub_ss;
			public IMemento SubSub_values;

			#region IMemento Members

			public object Target { get { return BaseMemento.Target; } }
			public bool IsRestored
			{
				get { return BaseMemento.IsRestored; }
				set { BaseMemento.IsRestored = value; }
			}

			#endregion
		}

		public SubWithPrimitiveArray() {}
		public SubWithPrimitiveArray(int f, string f1, double f2, int s, string s1, Obj owner, int ss)
			: base(f, f1, f2, s, s1, owner)
		{
		}

		public int SS { get { return _ss; } set { _ss = value; } }
		public List<int> Values { get { return new List<int>(_values); } }
		public void AddValue(int value)
		{
			int[] v = new int[_values.Length + 1];
			Array.Copy(_values, v, _values.Length);
			v[_values.Length] = value;
			_values = v;
		}

		[MementoCapture]
		static IMemento MementoCapture(IMementoContext ctx, SubWithPrimitiveArray item)
		{
			SubSubMemento m;
			m.BaseMemento = ATTi.Core.Mementos.Memento.CaptureBaseMemento<Sub>(ctx, item);
			m.SubSub_Blibber = ATTi.Core.Mementos.Memento.CaptureMemento<Obj>(ctx, Blibber);
			m.SubSub_ss = item._ss;
			m.SubSub_values = ATTi.Core.Mementos.Memento.CaptureArrayMemento<int>(ctx, item._values);
			return m;
		}

		[MementoRestore]
		static void MementoRestore(IMementoContext ctx, ref SubWithPrimitiveArray item, object m)
		{
			SubSubMemento data = (SubSubMemento)m;			
			Sub b = item;
			ATTi.Core.Mementos.Memento.RestoreBaseMemento<Sub>(ctx, ref b, data.BaseMemento);
			ATTi.Core.Mementos.Memento.RestoreMemento<Obj>(ctx, ref Blibber, data.SubSub_Blibber);
			item._ss = data.SubSub_ss;
			ATTi.Core.Mementos.Memento.RestoreArrayMemento<int>(ctx, ref item._values, data.SubSub_values);
		}
	}

	public class SubWithArrayOfObjects : Sub
	{
		int _ss;
		object[] _values = new object[0];

		public struct SubWithArrayOfObjectsMemento : IMemento
		{
			public IMemento BaseMemento;
			public int SubSub_ss;
			public IMemento SubSub_values;

			#region IMemento Members

			public object Target { get { return BaseMemento.Target; } }
			public bool IsRestored
			{
				get { return BaseMemento.IsRestored; }
				set { BaseMemento.IsRestored = value; }
			}

			#endregion
		}

		public SubWithArrayOfObjects() { }
		public SubWithArrayOfObjects(int f, string f1, double f2, int s, string s1, Obj owner, int ss)
			: base(f, f1, f2, s, s1, owner)
		{
		}

		public int SS { get { return _ss; } set { _ss = value; } }
		public List<object> Values { get { return new List<object>(_values); } }
		public void AddValue(object value)
		{
			object[] v = new object[_values.Length + 1];
			Array.Copy(_values, v, _values.Length);
			v[_values.Length] = value;
			_values = v;
		}

		[MementoCapture]
		static IMemento MementoCapture(IMementoContext ctx, SubWithArrayOfObjects item)
		{
			SubWithArrayOfObjectsMemento m;
			m.BaseMemento = ATTi.Core.Mementos.Memento.CaptureBaseMemento<Sub>(ctx, item);
			m.SubSub_ss = item._ss;
			m.SubSub_values = ATTi.Core.Mementos.Memento.CaptureArrayMemento<object>(ctx, item._values);
			return m;
		}

		[MementoRestore]
		static void MementoRestore(IMementoContext ctx, ref SubWithArrayOfObjects item, object m)
		{
			SubWithArrayOfObjectsMemento data = (SubWithArrayOfObjectsMemento)m;
			Sub b = item;
			ATTi.Core.Mementos.Memento.RestoreBaseMemento<Sub>(ctx, ref b, data.BaseMemento);
			item._ss = data.SubSub_ss;
			ATTi.Core.Mementos.Memento.RestoreArrayMemento<object>(ctx, ref item._values, data.SubSub_values);
		}
	}

	public class SubWithListField : Sub
	{
		List<object> _values = new List<object>();

		public struct SubWithListFieldMemento : IMemento
		{
			public IMemento BaseMemento;
			public IMemento SubSub_values;

			#region IMemento Members

			public object Target { get { return BaseMemento.Target; } }
			public bool IsRestored
			{
				get { return BaseMemento.IsRestored; }
				set { BaseMemento.IsRestored = value; }
			}

			#endregion
		}

		public SubWithListField() { }
		public SubWithListField(int f, string f1, double f2, int s, string s1, Obj owner, int ss)
			: base(f, f1, f2, s, s1, owner)
		{
		}

		public List<object> Values { get { return _values; } }

		[MementoCapture]
		static IMemento MementoCapture(IMementoContext ctx, SubWithListField item)
		{
			SubWithListFieldMemento m;
			m.BaseMemento = ATTi.Core.Mementos.Memento.CaptureBaseMemento<Sub>(ctx, item);
			m.SubSub_values = ATTi.Core.Mementos.Memento.CaptureMemento(ctx, item._values);
			return m;
		}

		[MementoRestore]
		static void MementoRestore(IMementoContext ctx, ref SubWithListField item, object m)
		{
			SubWithListFieldMemento data = (SubWithListFieldMemento)m;
			Sub b = item;
			ATTi.Core.Mementos.Memento.RestoreBaseMemento<Sub>(ctx, ref b, data.BaseMemento);
			ATTi.Core.Mementos.Memento.RestoreMemento(ctx, ref item._values, data.SubSub_values);
		}
	}
}
	