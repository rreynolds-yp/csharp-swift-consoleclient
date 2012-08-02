namespace ATTi.Core.Contracts
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Static class containing precondition helper methods.
	/// </summary>
	public static class Require
	{
		

		const ContractType Contract = ContractType.Precondition;

		

		

		/// <summary>
		/// Asserts that the state value given is true.
		/// </summary>
		/// <param name="value">A value to assert.</param>
		/// <param name="errorStateMessage">An error message thrown if the assertion fails</param>
		public static void AssertState(bool value, string errorStateMessage)
		{
			if (!value)
				ContractUtil.ThrowContractException(errorStateMessage, Contract);
		}

		public static void AssertState(bool value, Func<string> errorGen)
		{
			if (!value)
				ContractUtil.ThrowContractException(errorGen(), Contract);
		}

		public static void AssertState(string valueName, bool value, string errorStateMessage)
		{
			if (!value)
				ContractUtil.ThrowContractException(String.Concat(valueName, " ", errorStateMessage), Contract);
		}

		public static void AssertState(string valueName, bool value, Func<string> errorGen)
		{
			if (!value)
				ContractUtil.ThrowContractException(String.Concat(valueName, " ", errorGen()), Contract);
		}

		public static void AssertState<V>(V value, Func<V, bool> stateCheck, string errorStateMessage)
		{
			ContractUtil.AssertState<V>(Contract, value, stateCheck, errorStateMessage);
		}

		public static void AssertState<V>(V value, Func<V, bool> stateCheck, Func<string> errorGen)
		{
			ContractUtil.AssertState<V>(Contract, value, stateCheck, errorGen);
		}

		public static void AtLeastOneParam<T>(string valueName, T[] expected)
		{
			ContractUtil.AtLeastOneParam<T>(Contract, valueName, expected);
		}

		public static void ExpectNotOneOf<T>(string valueName, T value, params T[] expected)
		{
			ContractUtil.ExpectNotOneOf<T>(Contract, valueName, value, expected);
		}

		public static void ExpectOneOf<T>(string valueName, T value, params T[] expected)
		{
			ContractUtil.ExpectOneOf<T>(Contract, valueName, value, expected);
		}

		/// <summary>
		/// Specifies and asserts that the value is a generic type definition.
		/// </summary>			
		/// <param name="value">The value</param>
		public static void GenericTypeDefinitionHasNArguments(System.Type value, int expectedN)
		{
			ContractUtil.GenericTypeDefinitionHasNArguments(Contract, value, expectedN);
		}

		public static void IsAssignableFrom<T>(string valueName, System.Type value)
		{
			ContractUtil.IsAssignableFrom<T>(Contract, valueName, value);
		}

		public static void IsAssignableFrom(Type assignableFrom, string valueName, Type value)
		{
			ContractUtil.IsAssignableFrom(Contract, assignableFrom, valueName, value);
		}

		public static void IsBetween<C>(C value, C lower, C upper)
			where C : IComparable
		{
			ContractUtil.IsBetween<C>(Contract, value, lower, upper);
		}

		public static void IsBetween<C>(string valueName, C value, C lower, C upper)
			where C : IComparable
		{
			ContractUtil.IsBetween<C>(Contract, value, lower, upper);
		}

		public static void IsBetweenInclusive<C>(C value, C lower, C upper)
			where C : IComparable
		{
			ContractUtil.IsBetweenInclusive<C>(Contract, value, lower, upper);
		}

		public static void IsBetweenInclusive<C>(string valueName, C value, C lower, C upper)
			where C : IComparable
		{
			ContractUtil.IsBetweenInclusive<C>(Contract, value, lower, upper);
		}

		/// <summary>
		/// Specifies and asserts that a value is equal to the default value for the type.
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="value">The value</param>
		public static void IsDefault<V>(V value)
		{
			ContractUtil.IsDefault<V>(Contract, value);
		}

		/// <summary>
		/// Specifies and asserts that a value is equal to the default value for the type.
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="valueName">The value's name.</param>
		/// <param name="value">The value</param>
		public static void IsDefault<V>(string valueName, V value)
		{
			ContractUtil.IsDefault<V>(Contract, valueName, value);
		}

		/// <summary>
		/// Specifies and asserts that the value is equal to the comparand.
		/// </summary>
		/// <param name="comparand">The comparand</param>
		/// <param name="value">The actual value</param>
		public static void IsEqual<C>(C comparand, C value)
			where C : IComparable
		{
			ContractUtil.IsEqual<C>(Contract, comparand, value);
		}

		/// <summary>
		/// Specifies and asserts that the value is a generic type.
		/// </summary>			
		/// <param name="value">The value</param>
		public static void IsGenericType(System.Type value)
		{
			ContractUtil.IsGenericType(Contract, value);
		}

		/// <summary>
		/// Specifies and asserts that the value is a generic type definition.
		/// </summary>			
		/// <param name="value">The value</param>
		public static void IsGenericTypeDefinition(System.Type value)
		{
			ContractUtil.IsGenericTypeDefinition(Contract, value);
		}

		/// <summary>
		/// Specifies and asserts that the value is a generic type definition.
		/// </summary>			
		/// <param name="valueName">The name of the value.</param>
		/// <param name="value">The value</param>
		public static void IsGenericTypeDefinition(string valueName, System.Type value)
		{
			ContractUtil.IsGenericTypeDefinition(Contract, valueName, value);
		}

		/// <summary>
		/// Specifies and asserts that the value is greater than the comparand.
		/// </summary>
		/// <param name="comparand">The comparand</param>
		/// <param name="value">The actual value</param>
		public static void IsGreaterThan<C>(C comparand, C value)
			where C : IComparable
		{
			ContractUtil.IsGreaterThan<C>(Contract, comparand, value);
		}

		/// <summary>
		/// Specifies and asserts that the value is greater than the comparand.
		/// </summary>
		/// <param name="valueName">The name of the value</param>
		/// <param name="comparand">The comparand</param>
		/// <param name="value">The actual value</param>
		public static void IsGreaterThan<C>(string valueName, C comparand, C value)
			where C : IComparable
		{
			ContractUtil.IsGreaterThan<C>(Contract, valueName, comparand, value);
		}

		/// <summary>
		/// Specifies and asserts that the value is greater than or equal to the comparand.
		/// </summary>
		/// <param name="comparand">The comparand</param>
		/// <param name="value">The actual value</param>
		public static void IsGreaterThanOrEqual<C>(C comparand, C value)
			where C : IComparable
		{
			ContractUtil.IsGreaterThanOrEqual<C>(Contract, comparand, value);
		}

		/// <summary>
		/// Specifies and asserts that the value is greater than or equal to the comparand.
		/// </summary>
		/// <param name="valueName">The name of the value being compared.</param>
		/// <param name="comparand">The comparand</param>
		/// <param name="value">The actual value</param>
		public static void IsGreaterThanOrEqual<C>(string valueName, C comparand, C value)
			where C : IComparable
		{
			ContractUtil.IsGreaterThanOrEqual<C>(Contract, valueName, comparand, value);
		}

		///<summary>
		/// Specifies and asserts that the instance is of type T.
		/// </summary>
		/// <typeparam name="T">The comparand's type T</typeparam>
		/// <param name="value">An instance of type V</param>
		public static void IsInstanceOfType<T>(object value)
		{
			ContractUtil.IsInstanceOfType<T>(Contract, value);
		}

		/// <summary>
		/// Specifies and asserts that the instance is of type T.
		/// </summary>
		/// <typeparam name="T">The comparand's type T</typeparam>
		/// <param name="value">An instance of type V</param>
		public static void IsInstanceOfType<T>(string valueName, object value)
		{
			ContractUtil.IsInstanceOfType<T>(Contract, valueName, value);
		}

		/// <summary>
		/// Specifies and asserts that the instance of type V is of type T.
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="comparandType">The comparand's type.</param>
		/// <param name="value">An instance of type V</param>
		public static void IsInstanceOfType<V>(System.Type comparandType, V value)
		{
			ContractUtil.IsInstanceOfType(Contract, comparandType, value);
		}

		/// <summary>
		/// Specifies and asserts that the value is less than the comparand.
		/// </summary>
		/// <param name="comparand">The comparand</param>
		/// <param name="value">The actual value</param>
		public static void IsLessThan<C>(C comparand, C value)
			where C : IComparable
		{
			ContractUtil.IsLessThan<C>(Contract, comparand, value);
		}

		public static void IsLessThan<C>(string valueName, C comparand, C value)
			where C : IComparable
		{
			ContractUtil.IsLessThan<C>(Contract, valueName, comparand, value);
		}

		/// <summary>
		/// Specifies and asserts that the value is less than or equal to the comparand.
		/// </summary>
		/// <param name="comparand">The comparand</param>
		/// <param name="value">The actual value</param>
		public static void IsLessThanOrEqual<C>(C comparand, C value)
			where C : IComparable
		{
			ContractUtil.IsLessThanOrEqual<C>(Contract, comparand, value);
		}

		public static void IsLessThanOrEqual<C>(string valueName, C comparand, C value)
			where C : IComparable
		{
			ContractUtil.IsLessThanOrEqual<C>(Contract, valueName, comparand, value);
		}

		/// <summary>
		/// Specifies and asserts that a value cannot equal the default value of type V.
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="value">The value</param>
		public static void IsNotDefault<V>(V value)
		{
			ContractUtil.IsNotDefault<V>(Contract, value);
		}

		/// <summary>
		/// Specifies and asserts that a value cannot equal the default value of type V.
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="valueName">The value's name</param>
		/// <param name="value">The value</param>
		public static void IsNotDefault<V>(string valueName, V value)
		{
			ContractUtil.IsNotDefault<V>(Contract, valueName, value);
		}

		/// <summary>
		/// Specifies and asserts that a value cannot equal the default value of type V.
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="valueName">The value's name</param>
		/// <param name="value">The value</param>
		/// <param name="customErrorMessage">A customized error message</param>
		public static void IsNotDefault<V>(string valueName, V value, string customErrorMessage)
		{
			ContractUtil.IsNotDefault<V>(Contract, valueName, value, customErrorMessage);
		}

		/// <summary>
		/// Specifies and asserts that a value cannot be empty. This contract is applicable
		/// to enumerable objects.
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="value">The value, as an IEnumerable&lt;value></param>
		public static void IsNotEmpty<V>(IEnumerable<V> value)
		{
			ContractUtil.IsNotEmpty<V>(Contract, value);
		}

		/// <summary>
		/// Specifies and asserts that a value cannot be empty. 
		/// This contract is applicable to strings and implies IsNotNull.
		/// </summary>
		/// <param name="value">The value</param>
		public static void IsNotEmpty(string value)
		{
			ContractUtil.IsNotEmpty(Contract, value);
		}

		/// <summary>
		/// Specifies and asserts that a value cannot be empty. 
		/// This contract is applicable to strings and implies IsNotNull.
		/// </summary>
		/// <param name="valueName">The value's name</param>
		/// <param name="value">The value</param>
		public static void IsNotEmpty(string valueName, string value)
		{
			ContractUtil.IsNotEmpty(Contract, valueName, value);
		}

		/// <summary>
		/// Specifies and asserts that the value is not equal to the comparand.
		/// </summary>
		/// <param name="comparand">The comparand</param>
		/// <param name="value">The actual value</param>
		public static void IsNotEqual<C>(C comparand, C value)
			where C : IComparable
		{
			ContractUtil.IsNotEqual<C>(Contract, comparand, value);
		}

		/// <summary>
		/// Specifies and asserts that a value is not equal to null.
		/// For value types use IsNotDefault().
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="value">The value</param>
		public static void IsNotNull<V>(V value)
		{
			ContractUtil.IsNotNull<V>(Contract, value);
		}

		/// <summary>
		/// Specifies and asserts that a value is not equal to null.
		/// For value types use IsNotDefault().
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="valueName">The name of the value</param>
		/// <param name="value">The value</param>
		public static void IsNotNull<V>(string valueName, V value)
		{
			ContractUtil.IsNotNull<V>(Contract, valueName, value);
		}

		/// <summary>
		/// Specifies and asserts that a value is not equal to null.
		/// For value types use IsNotDefault().
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="valueName">The name of the value</param>
		/// <param name="value">The value</param>
		/// <param name="customErrorMessage">A custom error message describing the contract</param>
		public static void IsNotNull<V>(string valueName, V value, string customErrorMessage)
		{
			ContractUtil.IsNotNull<V>(Contract, valueName, value, customErrorMessage);
		}

		/// <summary>
		/// Specifies and asserts that a value is equal to null.
		/// For value types use IsDefault().
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="value">The value</param>
		public static void IsNull<V>(V value)
		{
			ContractUtil.IsNull<V>(Contract, value);
		}

		/// <summary>
		/// Specifies and asserts that a named value is equal to null.
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="valueName">The value's name.</param>
		/// <param name="value">The value</param>
		public static void IsNull<V>(string valueName, V value)
		{
			ContractUtil.IsNull<V>(Contract, valueName, value);
		}

		public static void ThrowIfException(Exception e, string customErrorMesssage)
		{
			if (e != null)
				ContractUtil.ThrowContractException(customErrorMesssage, e, Contract);
		}

		
	}
}