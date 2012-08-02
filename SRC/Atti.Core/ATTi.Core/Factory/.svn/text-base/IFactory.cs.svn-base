using System;
using System.Collections.Generic;

namespace ATTi.Core.Factory
{
	/// <summary>
	/// Interface for class factories.
	/// </summary>
	public interface IFactory
	{
		/// <summary>
		/// Gets the factory's target type. This type may be an interface or an abstract type.
		/// </summary>
		Type TargetType { get; }
		/// <summary>
		/// Gets the factory's concrete type. This type will be assignable to the factory's target type
		/// and will be a concrete type.
		/// </summary>
		Type ConcreteType { get; }
		/// <summary>
		/// Creates an instance of the target type.
		/// </summary>
		/// <returns>An instance of the target type.</returns>
		object CreateUntyped();
		/// <summary>
		/// Creates a named instance of the targe type.
		/// </summary>
		/// <param name="name">The name of the desired instance.</param>
		/// <returns>The named instance.</returns>
		/// <exception cref="ArgumentOutOfRangeException">thrown if the named instance is not available to the factory.</exception>
		object CreateUntyped(string name);
		/// <summary>
		/// Gets instances of the type that are kept by the factory. In most cases this will be the
		/// named instances. If a factory implements a caching strategy it also may include cached
		/// instances.
		/// </summary>
		/// <returns>The instances kept by the factory.</returns>
		IEnumerable<object> GetUntypedInstances();
	}

	/// <summary>
	/// Generic IFactory interface.
	/// </summary>
	/// <typeparam name="T">Target type of the factory.</typeparam>
	public interface IFactory<T> : IFactory
	{
		/// <summary>
		/// Creates a new instance of the target type.
		/// </summary>
		/// <returns>A new instance of the target type.</returns>
		T CreateInstance();
		/// <summary>
		/// Creates a named instance of the target type.
		/// </summary>
		/// <param name="name">The target instance's name.</param>
		/// <returns>The named instance.</returns>
		/// <exception cref="ArgumentOutOfRangeException">thrown if the named instance is not available to the factory.</exception>
		T CreateInstance(string name);
		/// <summary>
		/// Gets instances of the type that are kept by the factory. In most cases this will be the
		/// named instances. If a factory implements a caching strategy it also may include cached
		/// instances.
		/// </summary>
		/// <returns>The instances kept by the factory.</returns>
		IEnumerable<T> GetAllInstances();
		/// <summary>
		/// Event for monitoring instances issued from the factory. Use sparingly!
		/// </summary>
		event FactoryInstanceEvent<T> InstanceAction;
	}
	/// <summary>
	/// Specialized IFactory implementation for adapting a constructor with 1 arguments.
	/// The factory framework will implement this interface on factories that have constructors
	/// taking 1 arguments.
	/// </summary>
	/// <typeparam name="T">The factory's target type T.</typeparam>
	/// <typeparam name="A">Type A of the argument in the first position on the target constructor.</typeparam>
	public interface IFactory<T, A> : IFactory<T>
	{
	/// <summary>
		/// Creates an instance of the target type using a constructor with 1 argument where
		/// the target constructor takes arguments that are assignment compatible with the arguments
		/// given.
		/// </summary>
		/// <param name="a">An argument in the first position</param>
		/// <returns>A concrete instance of the factory's target type T</returns>
		T CreateInstanceWithInitialization(A a);
	}
	/// <summary>
	/// Specialized IFactory implementation for adapting a constructor with 2 arguments.
	/// The factory framework will implement this interface on factories that have constructors
	/// taking 2 arguments.
	/// </summary>
	/// <typeparam name="T">The factory's target type T.</typeparam>
	/// <typeparam name="A">Type A of the argument in the first position on the target constructor.</typeparam>
	/// <typeparam name="A1">Type A1 of the argument in the second position on the target constructor.</typeparam>
	public interface IFactory<T, A, A1> : IFactory<T>
	{
		/// <summary>
		/// Creates an instance of the target type using a constructor with 2 arguments where
		/// the target constructor takes arguments that are assignment compatible with the arguments
		/// given.
		/// </summary>
		/// <param name="a">An argument in the first position</param>
		/// <param name="a1">An argument in the second position</param>
		/// <returns>A concrete instance of the factory's target type T</returns>
		T CreateInstanceWithInitialization(A a, A1 a1);
	}
	/// <summary>
	/// Specialized IFactory implementation for adapting a constructor with 3 arguments.
	/// The factory framework will implement this interface on factories that have constructors
	/// taking 3 arguments.
	/// </summary>
	/// <typeparam name="T">The factory's target type T.</typeparam>
	/// <typeparam name="A">Type A of the argument in the first position on the target constructor.</typeparam>
	/// <typeparam name="A1">Type A1 of the argument in the second position on the target constructor.</typeparam>
	/// <typeparam name="A2">Type A2 of the argument in the thrird position on the target constructor.</typeparam>
	public interface IFactory<T, A, A1, A2> : IFactory<T>
	{
		/// <summary>
		/// Creates an instance of the target type using a constructor with 3 arguments where
		/// the target constructor takes arguments that are assignment compatible with the arguments
		/// given.
		/// </summary>
		/// <param name="a">An argument in the first position</param>
		/// <param name="a1">An argument in the second position</param>
		/// <param name="a2">An argument in the thrid position</param>
		/// <returns>A concrete instance of the factory's target type T</returns>
		T CreateInstanceWithInitialization(A a, A1 a1, A2 a2);
	}
	/// <summary>
	/// Specialized IFactory implementation for adapting a constructor with 4 arguments.
	/// The factory framework will implement this interface on factories that have constructors
	/// taking 4 arguments.
	/// </summary>
	/// <typeparam name="T">The factory's target type T.</typeparam>
	/// <typeparam name="A">Type A of the argument in the first position on the target constructor.</typeparam>
	/// <typeparam name="A1">Type A1 of the argument in the second position on the target constructor.</typeparam>
	/// <typeparam name="A2">Type A2 of the argument in the thrird position on the target constructor.</typeparam>
	/// <typeparam name="A3">Type A3 of the argument in the fourth position on the target constructor.</typeparam>
	public interface IFactory<T, A, A1, A2, A3> : IFactory<T>
	{
		/// <summary>
		/// Creates an instance of the target type using a constructor with 4 arguments where
		/// the target constructor takes arguments that are assignment compatible with the arguments
		/// given.
		/// </summary>
		/// <param name="a">An argument in the first position</param>
		/// <param name="a1">An argument in the second position</param>
		/// <param name="a2">An argument in the thrid position</param>
		/// <param name="a3">An argument in the foruth position</param>
		/// <returns>A concrete instance of the factory's target type T</returns>
		T CreateInstanceWithInitialization(A a, A1 a1, A2 a2, A3 a3);
	}
	/// <summary>
	/// Specialized IFactory implementation for adapting a constructor with 5 arguments.
	/// The factory framework will implement this interface on factories that have constructors
	/// taking 5 arguments.
	/// </summary>
	/// <typeparam name="T">The factory's target type T.</typeparam>
	/// <typeparam name="A">Type A of the argument in the first position on the target constructor.</typeparam>
	/// <typeparam name="A1">Type A1 of the argument in the second position on the target constructor.</typeparam>
	/// <typeparam name="A2">Type A2 of the argument in the thrird position on the target constructor.</typeparam>
	/// <typeparam name="A3">Type A3 of the argument in the fourth position on the target constructor.</typeparam>
	/// <typeparam name="A4">Type A4 of the argument in the fifth position on the target constructor.</typeparam>
	public interface IFactory<T, A, A1, A2, A3, A4> : IFactory<T>
	{
		/// <summary>
		/// Creates an instance of the target type using a constructor with 5 arguments where
		/// the target constructor takes arguments that are assignment compatible with the arguments
		/// given.
		/// </summary>
		/// <param name="a">An argument in the first position</param>
		/// <param name="a1">An argument in the second position</param>
		/// <param name="a2">An argument in the thrid position</param>
		/// <param name="a3">An argument in the foruth position</param>
		/// <param name="a4">An argument in the fifth position</param>
		/// <returns>A concrete instance of the factory's target type T</returns>
		T CreateInstanceWithInitialization(A a, A1 a1, A2 a2, A3 a3, A4 a4);
	}
	/// <summary>
	/// Specialized IFactory implementation for adapting a constructor with 6 arguments.
	/// The factory framework will implement this interface on factories that have constructors
	/// taking 6 arguments.
	/// </summary>
	/// <typeparam name="T">The factory's target type T.</typeparam>
	/// <typeparam name="A">Type A of the argument in the first position on the target constructor.</typeparam>
	/// <typeparam name="A1">Type A1 of the argument in the second position on the target constructor.</typeparam>
	/// <typeparam name="A2">Type A2 of the argument in the thrird position on the target constructor.</typeparam>
	/// <typeparam name="A3">Type A3 of the argument in the fourth position on the target constructor.</typeparam>
	/// <typeparam name="A4">Type A4 of the argument in the fifth position on the target constructor.</typeparam>
	/// <typeparam name="A5">Type A5 of the argument in the sixth position on the target constructor.</typeparam>
	public interface IFactory<T, A, A1, A2, A3, A4, A5> : IFactory<T>
	{
		/// <summary>
		/// Creates an instance of the target type using a constructor with 6 arguments where
		/// the target constructor takes arguments that are assignment compatible with the arguments
		/// given.
		/// </summary>
		/// <param name="a">An argument in the first position</param>
		/// <param name="a1">An argument in the second position</param>
		/// <param name="a2">An argument in the thrid position</param>
		/// <param name="a3">An argument in the foruth position</param>
		/// <param name="a4">An argument in the fifth position</param>
		/// <param name="a5">An argument in the sixth position</param>
		/// <returns>A concrete instance of the factory's target type T</returns>
		T CreateInstanceWithInitialization(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5);
	}
	/// <summary>
	/// Specialized IFactory implementation for adapting a constructor with 7 arguments.
	/// The factory framework will implement this interface on factories that have constructors
	/// taking 7 arguments.
	/// </summary>
	/// <typeparam name="T">The factory's target type T.</typeparam>
	/// <typeparam name="A">Type A of the argument in the first position on the target constructor.</typeparam>
	/// <typeparam name="A1">Type A1 of the argument in the second position on the target constructor.</typeparam>
	/// <typeparam name="A2">Type A2 of the argument in the thrird position on the target constructor.</typeparam>
	/// <typeparam name="A3">Type A3 of the argument in the fourth position on the target constructor.</typeparam>
	/// <typeparam name="A4">Type A4 of the argument in the fifth position on the target constructor.</typeparam>
	/// <typeparam name="A5">Type A5 of the argument in the sixth position on the target constructor.</typeparam>
	/// <typeparam name="A6">Type A6 of the argument in the seventh position on the target constructor.</typeparam>
	public interface IFactory<T, A, A1, A2, A3, A4, A5, A6> : IFactory<T>
	{
		/// <summary>
		/// Creates an instance of the target type using a constructor with 7 arguments where
		/// the target constructor takes arguments that are assignment compatible with the arguments
		/// given.
		/// </summary>
		/// <param name="a">An argument in the first position</param>
		/// <param name="a1">An argument in the second position</param>
		/// <param name="a2">An argument in the thrid position</param>
		/// <param name="a3">An argument in the foruth position</param>
		/// <param name="a4">An argument in the fifth position</param>
		/// <param name="a5">An argument in the sixth position</param>
		/// <param name="a6">An argument in the seventh position</param>
		/// <returns>A concrete instance of the factory's target type T</returns>
		T CreateInstanceWithInitialization(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6);
	}
	/// <summary>
	/// Specialized IFactory implementation for adapting a constructor with 8 arguments.
	/// The factory framework will implement this interface on factories that have constructors
	/// taking 8 arguments.
	/// </summary>
	/// <typeparam name="T">The factory's target type T.</typeparam>
	/// <typeparam name="A">Type A of the argument in the first position on the target constructor.</typeparam>
	/// <typeparam name="A1">Type A1 of the argument in the second position on the target constructor.</typeparam>
	/// <typeparam name="A2">Type A2 of the argument in the thrird position on the target constructor.</typeparam>
	/// <typeparam name="A3">Type A3 of the argument in the fourth position on the target constructor.</typeparam>
	/// <typeparam name="A4">Type A4 of the argument in the fifth position on the target constructor.</typeparam>
	/// <typeparam name="A5">Type A5 of the argument in the sixth position on the target constructor.</typeparam>
	/// <typeparam name="A6">Type A6 of the argument in the seventh position on the target constructor.</typeparam>
	/// <typeparam name="A7">Type A7 of the argument in the eigth position on the target constructor.</typeparam>
	public interface IFactory<T, A, A1, A2, A3, A4, A5, A6, A7> : IFactory<T>
	{
		/// <summary>
		/// Creates an instance of the target type using a constructor with 8 arguments where
		/// the target constructor takes arguments that are assignment compatible with the arguments
		/// given.
		/// </summary>
		/// <param name="a">An argument in the first position</param>
		/// <param name="a1">An argument in the second position</param>
		/// <param name="a2">An argument in the thrid position</param>
		/// <param name="a3">An argument in the foruth position</param>
		/// <param name="a4">An argument in the fifth position</param>
		/// <param name="a5">An argument in the sixth position</param>
		/// <param name="a6">An argument in the seventh position</param>
		/// <param name="a7">An argument in the eigth position</param>
		/// <returns>A concrete instance of the factory's target type T</returns>
		T CreateInstanceWithInitialization(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7);
	}
	/// <summary>
	/// Specialized IFactory implementation for adapting a constructor with 9 arguments.
	/// The factory framework will implement this interface on factories that have constructors
	/// taking 9 arguments.
	/// </summary>
	/// <typeparam name="T">The factory's target type T.</typeparam>
	/// <typeparam name="A">Type A of the argument in the first position on the target constructor.</typeparam>
	/// <typeparam name="A1">Type A1 of the argument in the second position on the target constructor.</typeparam>
	/// <typeparam name="A2">Type A2 of the argument in the thrird position on the target constructor.</typeparam>
	/// <typeparam name="A3">Type A3 of the argument in the fourth position on the target constructor.</typeparam>
	/// <typeparam name="A4">Type A4 of the argument in the fifth position on the target constructor.</typeparam>
	/// <typeparam name="A5">Type A5 of the argument in the sixth position on the target constructor.</typeparam>
	/// <typeparam name="A6">Type A6 of the argument in the seventh position on the target constructor.</typeparam>
	/// <typeparam name="A7">Type A7 of the argument in the eigth position on the target constructor.</typeparam>
	/// <typeparam name="A8">Type A8 of the argument in the ninth position on the target constructor.</typeparam>
	public interface IFactory<T, A, A1, A2, A3, A4, A5, A6, A7, A8> : IFactory<T>
	{
		/// <summary>
		/// Creates an instance of the target type using a constructor with 9 arguments where
		/// the target constructor takes arguments that are assignment compatible with the arguments
		/// given.
		/// </summary>
		/// <param name="a">An argument in the first position</param>
		/// <param name="a1">An argument in the second position</param>
		/// <param name="a2">An argument in the thrid position</param>
		/// <param name="a3">An argument in the foruth position</param>
		/// <param name="a4">An argument in the fifth position</param>
		/// <param name="a5">An argument in the sixth position</param>
		/// <param name="a6">An argument in the seventh position</param>
		/// <param name="a7">An argument in the eigth position</param>
		/// <param name="a8">An argument in the ninth position</param>
		/// <returns>A concrete instance of the factory's target type T</returns>
		T CreateInstanceWithInitialization(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8);
	}
	/// <summary>
	/// Specialized IFactory implementation for adapting a constructor with 10 arguments.
	/// The factory framework will implement this interface on factories that have constructors
	/// taking 10 arguments.
	/// </summary>
	/// <typeparam name="T">The factory's target type T.</typeparam>
	/// <typeparam name="A">Type A of the argument in the first position on the target constructor.</typeparam>
	/// <typeparam name="A1">Type A1 of the argument in the second position on the target constructor.</typeparam>
	/// <typeparam name="A2">Type A2 of the argument in the thrird position on the target constructor.</typeparam>
	/// <typeparam name="A3">Type A3 of the argument in the fourth position on the target constructor.</typeparam>
	/// <typeparam name="A4">Type A4 of the argument in the fifth position on the target constructor.</typeparam>
	/// <typeparam name="A5">Type A5 of the argument in the sixth position on the target constructor.</typeparam>
	/// <typeparam name="A6">Type A6 of the argument in the seventh position on the target constructor.</typeparam>
	/// <typeparam name="A7">Type A7 of the argument in the eigth position on the target constructor.</typeparam>
	/// <typeparam name="A8">Type A8 of the argument in the ninth position on the target constructor.</typeparam>
	/// <typeparam name="A9">Type A9 of the argument in the tenth position on the target constructor.</typeparam>
	public interface IFactory<T, A, A1, A2, A3, A4, A5, A6, A7, A8, A9> : IFactory<T>
	{
		/// <summary>
		/// Creates an instance of the target type using a constructor with 10 arguments where
		/// the target constructor takes arguments that are assignment compatible with the arguments
		/// given.
		/// </summary>
		/// <param name="a">An argument in the first position</param>
		/// <param name="a1">An argument in the second position</param>
		/// <param name="a2">An argument in the thrid position</param>
		/// <param name="a3">An argument in the foruth position</param>
		/// <param name="a4">An argument in the fifth position</param>
		/// <param name="a5">An argument in the sixth position</param>
		/// <param name="a6">An argument in the seventh position</param>
		/// <param name="a7">An argument in the eigth position</param>
		/// <param name="a8">An argument in the ninth position</param>
		/// <param name="a9">An argument in the tenth position</param>
		/// <returns>A concrete instance of the factory's target type T</returns>
		T CreateInstanceWithInitialization(A a, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9);
	}

}
