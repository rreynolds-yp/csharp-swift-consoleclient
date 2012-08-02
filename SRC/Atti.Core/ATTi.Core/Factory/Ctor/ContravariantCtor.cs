
namespace ATTi.Core.Factory.Ctor
{
	/// <summary>
	/// Implementation of the ICtor interface, used by the factory
	/// framework to achieve contravariance when choosing a constructor.
	/// </summary>
	/// <typeparam name="T">The target type. This is the type requested from the factory.</typeparam>
	/// <typeparam name="A0">Type of argument given to the factory in position zero during the create instance call.</typeparam>
	/// <typeparam name="A00">Type of argument accepted by a compatible constructor in position zero.</typeparam>
	internal class ContravariantCtor<T, A0, A00>
		: ICtor<T, A0>
			where A0 : A00
	{
		ICtor<T, A00> _adaptee;
		public ContravariantCtor(ICtor<T, A00> adaptee)
		{
			_adaptee = adaptee;
		}
		public T Construct(A0 a)
		{
			return _adaptee.Construct((A00)a);
		}
	}
	internal class ContravariantCtor<T, A0, A1, A00, A01>
	: ICtor<T, A0, A1>
		where A0 : A00
		where A1 : A01
	{
		ICtor<T, A00, A01> _adaptee;
		public ContravariantCtor(ICtor<T, A00, A01> adaptee)
		{
			_adaptee = adaptee;
		}
		public T Construct(A0 a, A1 a1)
		{
			return _adaptee.Construct((A00)a, (A01)a1);
		}
	}
	internal class ContravariantCtor<T, A0, A1, A2
													, A00, A01, A02>
	: ICtor<T, A0, A1, A2>
		where A0 : A00
		where A1 : A01
		where A2 : A02
	{
		ICtor<T, A00, A01, A02> _adaptee;
		public ContravariantCtor(ICtor<T, A00, A01, A02> adaptee)
		{
			_adaptee = adaptee;
		}
		public T Construct(A0 a, A1 a1, A2 a2)
		{
			return _adaptee.Construct((A00)a, (A01)a1, (A02)a2);
		}
	}
	internal class ContravariantCtor<T, A0, A1, A2, A3
													, A00, A01, A02, A03>
	: ICtor<T, A0, A1, A2, A3>
		where A0 : A00
		where A1 : A01
		where A2 : A02
		where A3 : A03
	{
		ICtor<T, A00, A01, A02, A03> _adaptee;
		public ContravariantCtor(ICtor<T, A00, A01, A02, A03> adaptee)
		{
			_adaptee = adaptee;
		}
		public T Construct(A0 a, A1 a1, A2 a2, A3 a3)
		{
			return _adaptee.Construct((A00)a, (A01)a1, (A02)a2, (A3)a3);
		}
	}
	internal class ContravariantCtor<T, A0, A1, A2, A3, A4
													, A00, A01, A02, A03, A04>
	: ICtor<T, A0, A1, A2, A3, A4>
		where A0 : A00
		where A1 : A01
		where A2 : A02
		where A3 : A03
		where A4 : A04
	{
		ICtor<T, A00, A01, A02, A03, A04> _adaptee;
		public ContravariantCtor(ICtor<T, A00, A01, A02, A03, A04> adaptee)
		{
			_adaptee = adaptee;
		}
		public T Construct(A0 a, A1 a1, A2 a2, A3 a3, A4 a4)
		{
			return _adaptee.Construct((A00)a, (A01)a1, (A02)a2, (A3)a3, (A4)a4);
		}
	}
	internal class ContravariantCtor<T, A0, A1, A2, A3, A4, A5
													, A00, A01, A02, A03, A04, A05>
	: ICtor<T, A0, A1, A2, A3, A4, A5>
		where A0 : A00
		where A1 : A01
		where A2 : A02
		where A3 : A03
		where A4 : A04
		where A5 : A05
	{
		ICtor<T, A00, A01, A02, A03, A04, A05> _adaptee;
		public ContravariantCtor(ICtor<T, A00, A01, A02, A03, A04, A05> adaptee)
		{
			_adaptee = adaptee;
		}
		public T Construct(A0 a, A1 a1, A2 a2, A3 a3, A4 a4
			, A5 a5)
		{
			return _adaptee.Construct((A00)a, (A01)a1, (A02)a2, (A3)a3, (A4)a4, (A5)a5);
		}
	}
	internal class ContravariantCtor<T, A0, A1, A2, A3, A4, A5, A6
													, A00, A01, A02, A03, A04, A05, A06>
	: ICtor<T, A0, A1, A2, A3, A4, A5, A6>
		where A0 : A00
		where A1 : A01
		where A2 : A02
		where A3 : A03
		where A4 : A04
		where A5 : A05
		where A6 : A06
	{
		ICtor<T, A00, A01, A02, A03, A04, A05, A06> _adaptee;
		public ContravariantCtor(ICtor<T, A00, A01, A02, A03, A04, A05, A06> adaptee)
		{
			_adaptee = adaptee;
		}
		public T Construct(A0 a, A1 a1, A2 a2, A3 a3, A4 a4
			, A5 a5, A6 a6)
		{
			return _adaptee.Construct((A00)a, (A01)a1, (A02)a2, (A3)a3, (A4)a4, (A5)a5, (A6)a6);
		}
	}
	internal class ContravariantCtor<T, A0, A1, A2, A3, A4, A5, A6, A7
													, A00, A01, A02, A03, A04, A05, A06, A07>
	: ICtor<T, A0, A1, A2, A3, A4, A5, A6, A7>
		where A0 : A00
		where A1 : A01
		where A2 : A02
		where A3 : A03
		where A4 : A04
		where A5 : A05
		where A6 : A06
		where A7 : A07
	{
		ICtor<T, A00, A01, A02, A03, A04, A05, A06, A07> _adaptee;
		public ContravariantCtor(ICtor<T, A00, A01, A02, A03, A04, A05, A06, A07> adaptee)
		{
			_adaptee = adaptee;
		}
		public T Construct(A0 a, A1 a1, A2 a2, A3 a3, A4 a4
			, A5 a5, A6 a6, A7 a7)
		{
			return _adaptee.Construct((A00)a, (A01)a1, (A02)a2, (A3)a3, (A4)a4, (A5)a5, (A6)a6, (A7)a7);
		}
	}
	internal class ContravariantCtor<T, A0, A1, A2, A3, A4, A5, A6, A7, A8
														, A00, A01, A02, A03, A04, A05, A06, A07, A08>
		: ICtor<T, A0, A1, A2, A3, A4, A5, A6, A7, A8>
		where A0 : A00
		where A1 : A01
		where A2 : A02
		where A3 : A03
		where A4 : A04
		where A5 : A05
		where A6 : A06
		where A7 : A07
		where A8 : A08
	{
		ICtor<T, A00, A01, A02, A03, A04, A05, A06, A07, A08> _adaptee;
		public ContravariantCtor(ICtor<T, A00, A01, A02, A03, A04, A05, A06, A07, A08> adaptee)
		{
			_adaptee = adaptee;
		}
		public T Construct(A0 a, A1 a1, A2 a2, A3 a3, A4 a4
			, A5 a5, A6 a6, A7 a7, A8 a8)
		{
			return _adaptee.Construct((A00)a, (A01)a1, (A02)a2, (A3)a3, (A4)a4, (A5)a5, (A6)a6, (A7)a7, (A8)a8);
		}
	}
	internal class ContravariantCtor<T, A0, A1, A2, A3, A4, A5, A6, A7, A8, A9
														, A00, A01, A02, A03, A04, A05, A06, A07, A08, A09>
		: ICtor<T, A0, A1, A2, A3, A4, A5, A6, A7, A8, A9>
		where A0 : A00
		where A1 : A01
		where A2 : A02
		where A3 : A03
		where A4 : A04
		where A5 : A05
		where A6 : A06
		where A7 : A07
		where A8 : A08
		where A9 : A09
	{
		ICtor<T, A00, A01, A02, A03, A04, A05, A06, A07, A08, A09> _adaptee;
		public ContravariantCtor(ICtor<T, A00, A01, A02, A03, A04, A05, A06, A07, A08, A09> adaptee)
		{
			_adaptee = adaptee;
		}
		public T Construct(A0 a, A1 a1, A2 a2, A3 a3, A4 a4
			, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9)
		{
			return _adaptee.Construct((A00)a, (A01)a1, (A02)a2, (A3)a3, (A4)a4, (A5)a5, (A6)a6, (A7)a7, (A8)a8, (A9)a9);
		}
	}
}
