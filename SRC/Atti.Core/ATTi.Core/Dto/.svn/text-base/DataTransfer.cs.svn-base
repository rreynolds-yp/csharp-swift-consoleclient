using System;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using ATTi.Core.Factory;

namespace ATTi.Core.Dto
{
	/// <summary>
	/// Utility class for working with data transfer objects.
	/// </summary>
	public static class DataTransfer
	{
		/// <summary>
		/// Loads a new instance of type T using the action given.
		/// </summary>
		/// <typeparam name="T">instance type T</typeparam>
		/// <param name="loader">An action that loads the state of the instance given.</param>
		/// <returns>A newly created instance of type T</returns>
		public static T Load<T>(Action<T> loader)
		{
			Contracts.Require.IsNotNull("loader", loader);

			T instance = Factory<T>.CreateImplInstance(DataTransfer<T>.ImplementationType);
			loader(instance);
			return instance;
		}

		/// <summary>
		/// Determines if the target object has extra data.
		/// </summary>
		/// <param name="target">The target object.</param>
		/// <param name="name">Name of the extra data.</param>
		/// <returns>True if the given named data is associated with the target object.</returns>
		public static bool HasExtraData(this object target, string name)
		{
			Contracts.Require.IsNotNull("target", target);
			Contracts.Require.IsNotNull("name", name);

			IDataTransferObject dto = target as IDataTransferObject;
			if (dto != null)
			{
				return dto.ExtraData.Contains(name);
			}
			return false;
		}

		/// <summary>
		/// Gets the extra named data associated with the target object.
		/// </summary>
		/// <typeparam name="V">Type V of the extra data.</typeparam>
		/// <param name="target">The target object.</param>
		/// <param name="key">Name of the extra data.</param>
		/// <returns>Extra named data associated with the target object.</returns>
		/// <exception cref="ArgumentOutOfRangeException">If there is not extra data
		/// associated with the target instance and the key given.</exception>
		public static V ExpectExtraData<V>(this object target, string key)
		{
			Contracts.Require.IsNotNull("target", target);
			Contracts.Require.IsNotNull("key", key);
			Contracts.Require.IsInstanceOfType<IDataTransferObject>(target);

			IDataTransferObject dto = target as IDataTransferObject;
			return dto.ExtraData.Expect<V>(key);
		}

		/// <summary>
		/// Gets the extra named data associated with the target object.
		/// </summary>
		/// <typeparam name="V">Type V of the extra data.</typeparam>
		/// <param name="target">The target object.</param>
		/// <param name="key">Name of the extra data.</param>
		/// <returns>Extra named data associated with the target object.</returns>
		public static bool TryGetExtraData<V>(this object target, string key, out V value)
		{
			Contracts.Require.IsNotNull("target", target);
			Contracts.Require.IsNotNull("key", key);
			Contracts.Require.IsInstanceOfType<IDataTransferObject>(target);

			IDataTransferObject dto = target as IDataTransferObject;
			return dto.ExtraData.TryGet<V>(key, out value);
		}

		/// <summary>
		/// Constructs a new instance of type T and populates it with
		/// data received in the XML element given.
		/// </summary>
		/// <typeparam name="T">instance type T</typeparam>
		/// <param name="item">An XElement containing incoming data for the instance.</param>
		/// <returns>An instance of type T populated with data from the XML <paramref name="item"/> given.</returns>
		public static T FromXml<T>(XElement item)
		{
			return DataTransfer<T>.FromXml(item);
		}

		/// <summary>
		/// Constructs a new instance of type T and populates it with
		/// data received in the XML element given. Extra properties and elements
		/// present in the XML will be captured as extra data if supported by the
		/// implementation of type T.
		/// </summary>
		/// <typeparam name="T">instance type T</typeparam>
		/// <param name="item">An XElement containing incoming data for the instance.</param>
		/// <returns>An instance of type T populated with data from the XML <paramref name="item"/> given.</returns>
		public static T FromXmlWithCaptureExtraData<T>(XElement item)
		{
			return DataTransfer<T>.FromXmlWithCaptureExtraData(item);
		}

		/// <summary>
		/// Constructs a new instance of type T and populates it with
		/// data received in the JSON object given.
		/// </summary>
		/// <typeparam name="T">instance type T</typeparam>
		/// <param name="item">A JSON object containing incoming data for the instance.</param>
		/// <returns>An instance of type T populated with data from the JSON <paramref name="item"/> given.</returns>
		public static T FromJson<T>(JObject item)
		{
			return DataTransfer<T>.FromJson(item);
		}

		public static T FromJsonWithCaptureExtraData<T>(JObject item)
		{
			return DataTransfer<T>.FromJsonWithCaptureExtraData(item);
		}
	}

}
