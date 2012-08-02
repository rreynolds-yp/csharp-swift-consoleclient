using System.Threading;

namespace ATTi.Core.Tests.Factory.Classes
{
	public class ClassWithDefaultConstructor : IClassWithID
	{
		static int __idGen = 0;

		public ClassWithDefaultConstructor()
		{
			ID = Interlocked.Increment(ref __idGen);
		}

		public int ID { get; private set; }
	}
}
