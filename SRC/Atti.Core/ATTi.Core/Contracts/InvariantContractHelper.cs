using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ATTi.Core.Reflection;

namespace ATTi.Core.Contracts
{

	internal static class InvariantContractHelper<T>
	{
		static bool __hasInvariantContract;
		static Action<T> __invoker;

		#region Constructors

		static InvariantContractHelper()
		{
			__hasInvariantContract = (from m in typeof(T).GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
																where m.IsDefined(typeof(InvariantContractAttribute), true)
																select m).SingleOrDefault() != null;
		}

		#endregion Constructors

		internal static bool HasInvariantContract
		{
			get { return __hasInvariantContract; }
		}

		internal static void InvokeContract(T target)
		{
			if (__hasInvariantContract)
			{
				if (__invoker == null)
				{
					lock (typeof(T).GetSafeAssemblyLock())
					{
						if (__invoker == null)
						{
							__invoker = GenerateInvoker();
						}
					}
				}
				__invoker(target);
			}
		}

		private static Action<T> GenerateInvoker()
		{
			DynamicMethod method = new DynamicMethod("InvokeInvariantContract"
					, null
					, new Type[] { typeof(T) }
					, typeof(T)
					);
			ILGenerator gen = method.GetILGenerator();

			MethodInfo inv = (from m in typeof(T).GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
							where m.IsDefined(typeof(InvariantContractAttribute), true)
							select m).SingleOrDefault();
			if (inv != null)
			{
				gen.Emit(OpCodes.Ldarg_0);
				gen.Emit(OpCodes.Call, inv);
			}
			gen.Emit(OpCodes.Ret);

			return (Action<T>)method.CreateDelegate(typeof(Action<T>));
		}

		
	}
}