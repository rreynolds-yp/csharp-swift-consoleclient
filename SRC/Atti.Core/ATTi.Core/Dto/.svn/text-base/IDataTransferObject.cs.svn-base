using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATTi.Core.Collections;

namespace ATTi.Core.Dto
{
	/// <summary>
	/// Interface for data transfer objects supporting extra data.
	/// </summary>
	public interface IDataTransferObject
	{
		IDataContainer<string> ExtraData { get; }
	}

	namespace SPI
	{
		/// <summary>
		/// SPI interface for data transfer objects.
		/// </summary>
		/// <remarks>
		/// If an object implements this interface then the framework
		/// will smuggle extra data received at the boundary with the
		/// object.
		/// </remarks>
		public interface IDataTransferObjectSPI : IDataTransferObject
		{
			void AddExtraData<V>(string name, V value);
			void SetExtraData(IDataContainer<string> extraData);
		}
	}
}
