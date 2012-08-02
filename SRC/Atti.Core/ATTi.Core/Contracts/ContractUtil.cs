namespace ATTi.Core.Contracts
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;
	using System.Text;

	using ATTi.Core.Properties;

	/// <summary>
	/// Static class containing contracts and helper methods for contracts.
	/// </summary>
	public static class ContractUtil
	{
		

		public static void AssertState<V>(ContractType c, V value, Func<V, bool> stateCheck, Func<string> genError)
		{
			if (stateCheck == null)
				ThrowContractException(String.Concat("stateCheck ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);
			if (genError == null)
				ThrowContractException(String.Concat("errorStateMessage ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);

			if (!stateCheck(value))
				ThrowContractException(genError(), c);
		}

		public static void AssertState<V>(ContractType c, V value, Func<V, bool> stateCheck, string errorStateMessage)
		{
			if (stateCheck == null)
				ThrowContractException(String.Concat("stateCheck ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);
			if (errorStateMessage == null)
				ThrowContractException(String.Concat("errorStateMessage ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);
			if (errorStateMessage.Length == 0)
				ThrowContractException(String.Concat("errorStateMessage ", Resources.ContractViolation_CannotBeEmpty), ContractType.Precondition);

			if (!stateCheck(value))
				ThrowContractException(errorStateMessage, c);
		}

		public static void AtLeastOneParam<T>(ContractType c, string valueName, T[] expected)
		{
			if (valueName == null)
				ThrowContractException(String.Concat("valueName ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);

			if (expected.Length == 0)
				ContractUtil.ThrowContractException(Resources.ContractViolation_ParamsParameterMustHaveAtLeastOneValue, c);
		}

		public static void ExpectNotOneOf<T>(ContractType c, string valueName, T value, params T[] notOneOf)
		{
			if (valueName == null)
				ThrowContractException(String.Concat("valueName ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);
			AtLeastOneParam(ContractType.Precondition, "expected", notOneOf);

			if (IsOneOf(value, notOneOf))
				ContractUtil.ThrowContractException(String.Concat(valueName, " ", String.Format(Resources.ContractViolation_StateIsInvalid_ExpectedNotOneOf, value,
					ContractUtil.GenerateStringContainingListOfValidStates<T>(notOneOf)))
					, c);
		}

		public static void ExpectOneOf<T>(ContractType c, string valueName, T value, params T[] expected)
		{
			if (valueName == null)
				ThrowContractException(String.Concat("valueName ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);
			AtLeastOneParam(ContractType.Precondition, "expected", expected);

			if (!IsOneOf(value, expected))
				ContractUtil.ThrowContractException(String.Concat(valueName, " ", String.Format(Resources.ContractViolation_StateIsInvalid_ExpectedOneOf, value,
					ContractUtil.GenerateStringContainingListOfValidStates<T>(expected)))
					, c);
		}

		/// <summary>
		/// Specifies and asserts that the value is a generic type definition.
		/// </summary>			
		/// <param name="value">The value</param>
		public static void GenericTypeDefinitionHasNArguments(ContractType c, System.Type value, int expectedN)
		{
			IsGenericTypeDefinition(ContractType.Precondition, value);
			System.Type[] argTypes = value.GetGenericArguments();
			if (argTypes.Length != expectedN)
				ThrowContractException(String.Format(Resources.ContractViolation_InvalidNumberOfTypeArguments, value.FullName)
					, c);
		}

		public static void IsAssignableFrom<T>(ContractType c, string valueName, System.Type value)
		{
			if (valueName == null)
				ThrowContractException(String.Concat("valueName ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);

			if (!typeof(T).IsAssignableFrom(value))
				ContractUtil.ThrowContractException(
					String.Format(Properties.Resources.ContractViolation_MustBeAssignableFromType, typeof(T).FullName),
					c
					);
		}

		public static void IsAssignableFrom(ContractType c, Type assignableFrom, string valueName, Type value)
		{
			if (assignableFrom == null)
				ThrowContractException(String.Concat("assignableFrom ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);
			if (valueName == null)
				ThrowContractException(String.Concat("valueName ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);

			if (!assignableFrom.IsAssignableFrom(value))
				ContractUtil.ThrowContractException(
					String.Format(Properties.Resources.ContractViolation_MustBeAssignableFromType, assignableFrom.FullName),
					c
					);
		}

		public static void IsBetween<C>(ContractType c, C value, C lower, C upper)
			where C : IComparable
		{
			Comparer<C> comparer = Comparer<C>.Default;
			if (comparer.Compare(value, lower) > 0
				&& comparer.Compare(value, upper) < 0)
				ThrowContractException(String.Format(Properties.Resources.ContractViolation_ValueMustBeBetween
					, lower, upper, value), c);
		}

		public static void IsBetween<C>(ContractType c, string valueName, C value, C lower, C upper)
			where C : IComparable
		{
			if (valueName == null)
				ThrowContractException(String.Concat("valueName ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);
			Comparer<C> comparer = Comparer<C>.Default;
			if (comparer.Compare(value, lower) > 0
				&& comparer.Compare(value, upper) < 0)
				ThrowContractException(String.Format(Properties.Resources.ContractViolation_ValueMustBeBetween
					, lower, upper, value), c);
		}

		public static void IsBetweenInclusive<C>(ContractType c, C value, C lower, C upper)
			where C : IComparable
		{
			Comparer<C> comparer = Comparer<C>.Default;
			if (comparer.Compare(value, lower) < 0
				&& comparer.Compare(value, upper) > 0)
				ThrowContractException(String.Format(Properties.Resources.ContractViolation_ValueMustBeBetweenInclusive
					, lower, upper, value), ContractType.Precondition);
		}

		public static void IsBetweenInclusive<C>(ContractType c, string valueName, C value, C lower, C upper)
			where C : IComparable
		{
			if (valueName == null)
				ThrowContractException(String.Concat("valueName ", Resources.ContractViolation_CannotBeNull), c);

			Comparer<C> comparer = Comparer<C>.Default;
			if (comparer.Compare(value, lower) < 0
				&& comparer.Compare(value, upper) > 0)
				ThrowContractException(String.Format(Properties.Resources.ContractViolation_ValueMustBeBetweenInclusive
					, lower, upper, value), c);
		}

		/// <summary>
		/// Specifies and asserts that a value is equal to the default value for the type.
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="value">The value</param>
		public static void IsDefault<V>(ContractType c, V value)
		{
			if (EqualityComparer<V>.Default.Equals(default(V), value))
				ThrowContractException(Resources.ContractViolation_MustBeDefault, c);
		}

		/// <summary>
		/// Specifies and asserts that a value is equal to the default value for the type.
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="valueName">The value's name.</param>
		/// <param name="value">The value</param>
		public static void IsDefault<V>(ContractType c, string valueName, V value)
		{
			if (valueName == null)
				ThrowContractException(String.Concat("valueName ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);

			if (typeof(V).IsValueType)
				ThrowContractException(Resources.ContractViolation_ConsiderIsDefault, ContractType.Precondition);

			if (EqualityComparer<V>.Default.Equals(default(V), value))
				ThrowContractException(String.Concat(valueName, " ", Resources.ContractViolation_MustBeDefault), c);
		}

		/// <summary>
		/// Specifies and asserts that the value is equal to the comparand.
		/// </summary>
		/// <param name="comparand">The comparand</param>
		/// <param name="value">The actual value</param>
		public static void IsEqual<C>(ContractType c, C comparand, C value)
			where C : IComparable
		{
			if (Comparer<C>.Default.Compare(comparand, value) != 0)
				ContractUtil.ThrowContractException(String.Format(Resources.ContractViolation_MustBeEqualTo, comparand, value), c);
		}

		/// <summary>
		/// Specifies and asserts that the value is a generic type.
		/// </summary>			
		/// <param name="value">The value</param>
		public static void IsGenericType(ContractType c, System.Type value)
		{
			if (value == null)
				ThrowContractException(String.Concat("value ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);
			if (!value.IsGenericType)
				ContractUtil.ThrowContractException(Resources.ContractViolation_ValueMustBeGenericType, c);
		}

		/// <summary>
		/// Specifies and asserts that the value is a generic type definition.
		/// </summary>			
		/// <param name="value">The value</param>
		public static void IsGenericTypeDefinition(ContractType c, System.Type value)
		{
			if (value == null)
				ThrowContractException(String.Concat("value ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);
			if (!value.IsGenericTypeDefinition)
				ThrowContractException(Resources.ContractViolation_ValueMustBeGenericTypeDefinition, c);
		}

		/// <summary>
		/// Specifies and asserts that the value is a generic type definition.
		/// </summary>			
		/// <param name="valueName">The name of the value.</param>
		/// <param name="value">The value</param>
		public static void IsGenericTypeDefinition(ContractType c, string valueName, System.Type value)
		{
			if (valueName == null)
				ThrowContractException(String.Concat("valueName ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);
			if (value == null)
				ThrowContractException(String.Concat("value ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);
			if (!value.IsGenericTypeDefinition)
				ThrowContractException(String.Concat(valueName, " ", Resources.ContractViolation_ValueMustBeGenericTypeDefinition), c);
		}

		/// <summary>
		/// Specifies and asserts that the value is greater than the comparand.
		/// </summary>
		/// <param name="comparand">The comparand</param>
		/// <param name="value">The actual value</param>
		public static void IsGreaterThan<C>(ContractType c, C comparand, C value)
			where C : IComparable
		{
			if (Comparer<C>.Default.Compare(comparand, value) > 0)
				ContractUtil.ThrowContractException(String.Format(Resources.ContractViolation_MustBeGreaterThan, comparand, value), c);
		}

		public static void IsGreaterThan<C>(ContractType c, string valueName, C comparand, C value)
			where C : IComparable
		{
			if (Comparer<C>.Default.Compare(comparand, value) > 0)
				ContractUtil.ThrowContractException(String.Concat(valueName, " ",
					String.Format(Resources.ContractViolation_MustBeGreaterThan, comparand, value)), c);
		}

		/// <summary>
		/// Specifies and asserts that the value is greater than or equal to the comparand.
		/// </summary>
		/// <param name="comparand">The comparand</param>
		/// <param name="value">The actual value</param>
		public static void IsGreaterThanOrEqual<C>(ContractType c, C comparand, C value)
			where C : IComparable
		{
			if (Comparer<C>.Default.Compare(comparand, value) >= 0)
				ContractUtil.ThrowContractException(String.Format(Resources.ContractViolation_MustBeGreaterThanOrEqual, comparand, value), c);
		}

		public static void IsGreaterThanOrEqual<C>(ContractType c, string valueName, C comparand, C value)
			where C : IComparable
		{
			if (Comparer<C>.Default.Compare(comparand, value) >= 0)
				ContractUtil.ThrowContractException(String.Concat(valueName, ' '
					, String.Format(Resources.ContractViolation_MustBeGreaterThanOrEqual, comparand, value))
					, c
					);
		}

		///<summary>
		/// Specifies and asserts that the instance is of type T.
		/// </summary>
		/// <typeparam name="T">The comparand's type T</typeparam>
		/// <param name="value">An instance of type V</param>
		public static void IsInstanceOfType<T>(ContractType c, object value)
		{
			if (value == null)
				ThrowContractException(String.Concat("value ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);

			if (!typeof(T).IsInstanceOfType(value))
				ThrowContractException(String.Format(Resources.ContractViolation_MustBeInstanceOfType
					, typeof(T).FullName, value.GetType().FullName), c);
		}

		/// <summary>
		/// Specifies and asserts that the instance is of type T.
		/// </summary>
		/// <typeparam name="T">The comparand's type T</typeparam>
		/// <param name="value">An instance of type V</param>
		public static void IsInstanceOfType<T>(ContractType c, string valueName, object value)
		{
			if (value == null)
				ThrowContractException(String.Concat("value ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);

			if (valueName == null)
				ThrowContractException(String.Concat("valueName ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);

			if (!typeof(T).IsInstanceOfType(value))
				ThrowContractException(String.Format(String.Concat(valueName, " ", Resources.ContractViolation_MustBeInstanceOfType
					, typeof(T).FullName, value.GetType().FullName)), c);
		}

		/// <summary>
		/// Specifies and asserts that the instance of type V is of type T.
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="comparandType">The comparand's type.</param>
		/// <param name="value">An instance of type V</param>
		public static void IsInstanceOfType<V>(ContractType c, System.Type comparandType, V value)
		{
			if (comparandType == null)
				ThrowContractException(String.Concat("comparandType ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);
			if (value == null)
				ThrowContractException(String.Concat("value ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);

			if (comparandType.IsInstanceOfType(value))
				ThrowContractException(String.Format(Resources.ContractViolation_MustBeInstanceOfType
					, comparandType.FullName, typeof(V).FullName), c);
		}

		/// <summary>
		/// Specifies and asserts that the value is less than the comparand.
		/// </summary>
		/// <param name="comparand">The comparand</param>
		/// <param name="value">The actual value</param>
		public static void IsLessThan<C>(ContractType c, C comparand, C value)
			where C : IComparable
		{
			if (Comparer<C>.Default.Compare(comparand, value) >= 0)
				ContractUtil.ThrowContractException(String.Format(Resources.ContractViolation_MustBeLessThan, comparand, value), c);
		}

		public static void IsLessThan<C>(ContractType c, string valueName, C comparand, C value)
			where C : IComparable
		{
			if (Comparer<C>.Default.Compare(comparand, value) >= 0)
				ContractUtil.ThrowContractException(String.Concat(valueName, " ",
					String.Format(Resources.ContractViolation_MustBeLessThan, comparand, value)), c);
		}

		/// <summary>
		/// Specifies and asserts that the value is less than or equal to the comparand.
		/// </summary>
		/// <param name="comparand">The comparand</param>
		/// <param name="value">The actual value</param>
		public static void IsLessThanOrEqual<C>(ContractType c, C comparand, C value)
			where C : IComparable
		{
			if (Comparer<C>.Default.Compare(comparand, value) > 0)
				ContractUtil.ThrowContractException(String.Format(Resources.ContractViolation_MustBeLessThanOrEqual, comparand, value), c);
		}

		public static void IsLessThanOrEqual<C>(ContractType c, string valueName, C comparand, C value)
			where C : IComparable
		{
			if (Comparer<C>.Default.Compare(comparand, value) > 0)
				ContractUtil.ThrowContractException(String.Concat(valueName, " ",
					String.Format(Resources.ContractViolation_MustBeLessThanOrEqual, comparand, value)), c);
		}

		/// <summary>
		/// Specifies and asserts that a value cannot equal the default value for the parameter.
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="value">The value</param>
		public static void IsNotDefault<V>(ContractType c, V value)
		{
			if (Object.Equals(value, default(V)))
				ContractUtil.ThrowContractException(Resources.ContractViolation_CannotBeDefault, c);
		}

		public static void IsNotDefault<V>(ContractType c, string valueName, V value)
		{
			if (valueName == null)
				ThrowContractException(String.Concat("valueName ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);

			if (Object.Equals(value, default(V)))
				ContractUtil.ThrowContractException(String.Concat(valueName, " ", Resources.ContractViolation_CannotBeDefault), c);
		}

		public static void IsNotDefault<V>(ContractType c, string valueName, V value, string customErrorMessage)
		{
			if (valueName == null)
				ThrowContractException(String.Concat("valueName ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);

			if (typeof(V).IsValueType)
				ThrowContractException(Resources.ContractViolation_ConsiderIsNotDefault, ContractType.Precondition);

			if (Object.Equals(value, default(V)))
				ContractUtil.ThrowContractException(String.Concat(valueName, " ", Resources.ContractViolation_CannotBeDefault), c);
		}

		/// <summary>
		/// Specifies and asserts that a value cannot be empty. This contract is applicable
		/// to enumerable objects.
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="value">The value, as an IEnumerable&lt;value></param>
		public static void IsNotEmpty<V>(ContractType c, IEnumerable<V> value)
		{
			if (value == null)
				ThrowContractException(String.Concat("value ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);

			if (value.Count() == 0)
				ThrowContractException(Resources.ContractViolation_CannotBeEmpty, c);
		}

		/// <summary>
		/// Specifies and asserts that a value cannot be empty. 
		/// This contract is applicable to strings and implies IsNotNull.
		/// </summary>
		/// <param name="value">The value</param>
		public static void IsNotEmpty(ContractType c, string value)
		{
			if (value == null)
				ThrowContractException(String.Concat("value ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);

			if (value.Length == 0)
				ThrowContractException(Resources.ContractViolation_CannotBeEmpty, c);
		}

		/// <summary>
		/// Specifies and asserts that a value cannot be empty. 
		/// This contract is applicable to strings and implies IsNotNull.
		/// </summary>
		/// <param name="valueName">The name of the parameter.</param>
		/// <param name="value">The value</param>
		public static void IsNotEmpty(ContractType c, string valueName, string value)
		{
			if (valueName == null)
				ThrowContractException(String.Concat("valueName ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);
			if (value == null)
				ThrowContractException(String.Concat("value ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);

			if (value.Length == 0)
				ThrowContractException(String.Concat(valueName, " ", Resources.ContractViolation_CannotBeEmpty), c);
		}

		/// <summary>
		/// Specifies and asserts that the value is not equal to the comparand.
		/// </summary>
		/// <param name="comparand">The comparand</param>
		/// <param name="value">The actual value</param>
		public static void IsNotEqual<C>(ContractType c, C comparand, C value)
			where C : IComparable
		{
			if (Comparer<C>.Default.Compare(comparand, value) == 0)
				ContractUtil.ThrowContractException(String.Format(Resources.ContractViolation_MustNotBeEqualTo, comparand, value), c);
		}

		/// <summary>
		/// Specifies and asserts that a value is equal to null.
		/// For value types use IsDefault().
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="value">The value</param>
		public static void IsNull<V>(ContractType c, V value)
		{
			if (typeof(V).IsValueType)
				ThrowContractException(Resources.ContractViolation_ConsiderIsDefault, ContractType.Precondition);

			if (value != null)
				ThrowContractException(Resources.ContractViolation_MustBeNull, c);
		}

		/// <summary>
		/// Specifies and asserts that a named value is equal to null.
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="valueName">The value's name.</param>
		/// <param name="value">The value</param>
		public static void IsNull<V>(ContractType c, string valueName, V value)
		{
			if (valueName == null)
				ThrowContractException(String.Concat("valueName ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);

			if (typeof(V).IsValueType)
				ThrowContractException(Resources.ContractViolation_ConsiderIsDefault, ContractType.Precondition);

			if (value != null)
				ThrowContractException(String.Concat(valueName, " ", Resources.ContractViolation_MustBeNull), c);
		}

		internal static string GenerateStringContainingListOfValidStates<T>(T[] expected)
		{
			StringBuilder result = new StringBuilder(400);
			foreach (T item in expected)
			{
				result.AppendLine(item.ToString());
			}
			return result.ToString();
		}

		/// <summary>
		/// Specifies and asserts that a value is not equal to null.
		/// For value types use IsNotDefault().
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="value">The value</param>
		internal static void IsNotNull<V>(ContractType c, V value)
		{
			if (typeof(V).IsValueType)
				ThrowContractException(Resources.ContractViolation_ConsiderIsNotDefault, ContractType.Precondition);

			if (value == null)
				ThrowContractException(Resources.ContractViolation_CannotBeNull, c);
		}

		/// <summary>
		/// Specifies and asserts that a value is not equal to null.
		/// For value types use IsNotDefault().
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="valueName">The name of the value</param>
		/// <param name="value">The value</param>
		internal static void IsNotNull<V>(ContractType c, string valueName, V value)
		{
			if (valueName == null)
				ThrowContractException(String.Concat("valueName ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);

			if (typeof(V).IsValueType)
				ThrowContractException(Resources.ContractViolation_ConsiderIsNotDefault, ContractType.Precondition);

			if (value == null)
				ThrowContractException(String.Concat(valueName, " ", Resources.ContractViolation_CannotBeNull), c);
		}

		/// <summary>
		/// Specifies and asserts that a value is not equal to null.
		/// For value types use IsNotDefault().
		/// </summary>
		/// <typeparam name="V">The value's type V</typeparam>
		/// <param name="valueName">The name of the value</param>
		/// <param name="value">The value</param>
		/// <param name="customErrorMessage">A custom error message describing the contract</param>
		internal static void IsNotNull<V>(ContractType c, string valueName, V value, string customErrorMessage)
		{
			if (valueName == null)
				ThrowContractException(String.Concat("valueName ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);

			if (valueName == null)
				ThrowContractException(String.Concat("errorMessageFunc ", Resources.ContractViolation_CannotBeNull), ContractType.Precondition);

			if (typeof(V).IsValueType)
				ThrowContractException(Resources.ContractViolation_ConsiderIsNotDefault, ContractType.Precondition);

			if (value == null)
				ContractUtil.ThrowContractException(String.Concat(valueName, " ", customErrorMessage), c);
		}

		internal static bool IsOneOf<T>(T value, T[] expected)
		{
			IComparer<T> compare = Comparer<T>.Default;
			#region Avoids iterating if limited number of expected items.
			switch (expected.Length)
			{
				case 1:
					if (compare.Compare(value, expected[0]) == 0) return true;
					break;
				case 2:
					if (compare.Compare(value, expected[0]) == 0
						|| compare.Compare(value, expected[1]) == 0) return true;
					break;
				case 3:
					if (compare.Compare(value, expected[0]) == 0
						|| compare.Compare(value, expected[1]) == 0
						|| compare.Compare(value, expected[2]) == 0) return true;
					break;
				case 4:
					if (compare.Compare(value, expected[0]) == 0
						|| compare.Compare(value, expected[1]) == 0
						|| compare.Compare(value, expected[2]) == 0
						|| compare.Compare(value, expected[3]) == 0) return true;
					break;
				case 5:
					if (compare.Compare(value, expected[0]) == 0
						|| compare.Compare(value, expected[1]) == 0
						|| compare.Compare(value, expected[2]) == 0
						|| compare.Compare(value, expected[3]) == 0
						|| compare.Compare(value, expected[4]) == 0) return true;
					break;
				default:
					if (compare.Compare(value, expected[0]) == 0
						|| compare.Compare(value, expected[1]) == 0
						|| compare.Compare(value, expected[2]) == 0
						|| compare.Compare(value, expected[3]) == 0
						|| compare.Compare(value, expected[4]) == 0
						|| compare.Compare(value, expected[5]) == 0) return true;
					for (int i = 6; i < expected.Length; i++)
					{
						if (compare.Compare(value, expected[i]) == 0) return true;
					}
					break;
			}
			#endregion
			return false;
		}

		internal static void ThrowContractException(string message, ContractType contractType)
		{
			StackTrace st = new StackTrace(3, false);
			StackFrame[] frames = st.GetFrames();
			MethodBase offender = null;

			switch (contractType)
			{
				case ContractType.Precondition:
					throw new PreconditionContractException(String.Format(@"Precondition contract violation: '{0}'
			Declared by: {1}
			Called by: {2}", message
							 , DescribeMethod(frames[0].GetMethod())
							 , DescribeMethod(frames[1].GetMethod()))
						);
				case ContractType.Postcondition:
					throw new PostconditionContractException(String.Format(@"Postcondition contract violation: '{0}'
			Declared by: {1}", message
							 , DescribeMethod(frames[0].GetMethod()))
							 );
				case ContractType.Invariant:
					for (int i = 1; i < frames.Length; i++)
					{
						Type d = frames[i].GetMethod().DeclaringType;

						if (d != null && d != typeof(ContractUtil) && d != typeof(Invariant))
						{
							offender = frames[i].GetMethod();
							if (offender.IsDefined(typeof(InvariantContractAttribute), true))
							{
								offender = frames[i].GetMethod();
								break; 
							}
							break;
						}
					}
					throw new InvariantContractException(String.Format(@"Invariant contract violation: '{0}'
			Declared by: {1}
			Invariants checked by: {2}", message
							 , frames[0].GetMethod().DeclaringType
							 , DescribeMethod(offender))
							 );
				case ContractType.Intermediate:
					throw new IntermediateContractException(String.Format(@"Intermediate contract violation: '{0}'
			Declared by: {1}", message
							 , frames[0].GetMethod().DeclaringType)
							 );
				default: throw new ContractException(message);
			}
		}

		internal static void ThrowContractException(string message, Exception cause, ContractType contractType)
		{
			StackTrace st = new StackTrace(3, false);
			StackFrame[] frames = st.GetFrames();
			MethodBase offender = null;

			switch (contractType)
			{
				case ContractType.Precondition:
					throw new PreconditionContractException(String.Format(@"Precondition contract violation: '{0}'
			Declared by: {1}
			Called by: {2}", message
							 , DescribeMethod(frames[0].GetMethod())
							 , DescribeMethod(frames[1].GetMethod())), cause
						);
				case ContractType.Postcondition:
					throw new PostconditionContractException(String.Format(@"Postcondition contract violation: '{0}'
			Declared by: {1}", message
							 , DescribeMethod(frames[0].GetMethod())), cause
							 );
				case ContractType.Invariant:
					for (int i = 1; i < frames.Length; i++)
					{
						Type d = frames[i].GetMethod().DeclaringType;

						if (d != null && d != typeof(ContractUtil) && d != typeof(Invariant))
						{
							offender = frames[i].GetMethod();
							if (offender.IsDefined(typeof(InvariantContractAttribute), true))
							{
								offender = frames[i].GetMethod();
								break;								
							}
							break;
						}
					}
					throw new InvariantContractException(String.Format(@"Invariant contract violation: '{0}'
			Declared by: {1}
			Invariants checked by: {2}", message
							 , frames[0].GetMethod().DeclaringType
							 , DescribeMethod(offender)), cause
							 );
				case ContractType.Intermediate:
					throw new IntermediateContractException(String.Format(@"Intermediate contract violation: '{0}'
			Declared by: {1}", message
							 , frames[0].GetMethod().DeclaringType), cause
							 );
				default: throw new ContractException(message);
			}
		}

		static string DescribeMethod(MethodBase m)
		{
			if (m == null) return "-unidentified method-";

			StringBuilder buffer = new StringBuilder(200);
			buffer.Append(m.DeclaringType.FullName)
				.Append('.');
			if (m.IsConstructor)
			{
				if (m.IsStatic) buffer.Append("cctor");
				else buffer.Append("ctor");
			}
			else
			{
				buffer.Append(m.Name);
			}
			List<GenericArgs> genericArgs = new List<GenericArgs>();
			if (m.IsGenericMethod)
			{
				buffer.Append('[');
				foreach (Type t in m.GetGenericArguments())
				{
					GenericArgs g = new GenericArgs();
					g.Name = t.Name;
					g.TypeName = t.Name;
					buffer.Append(t.Name);
				}
				buffer.Append(']');
			}
			buffer.Append('(');
			foreach (ParameterInfo p in m.GetParameters())
			{
				if (p.Position > 0) buffer.Append(',');
				buffer.Append(p.ParameterType.Name).Append(' ').Append(p.Name);
			}
			buffer.Append(')');
			return buffer.ToString();
		}

		

		#region Nested Types

		struct GenericArgs
		{
			

			public string Name;
			public string TypeName;

			
		}

		#endregion Nested Types
	}
}