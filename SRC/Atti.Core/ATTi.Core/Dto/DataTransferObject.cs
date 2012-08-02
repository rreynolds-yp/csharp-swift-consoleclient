using ATTi.Core.Collections;
using ATTi.Core.Dto.SPI;

namespace ATTi.Core.Dto
{		
	/// <summary>
	/// Abstract implementation of the data transfer object interfaces.
	/// </summary>
	/// <remarks>
	/// When the framework automatically emits DTOs it will derive those
	/// types from this base class.
	/// </remarks>
	public abstract class DataTransferObject : IDataTransferObjectSPI
	{
		IDataContainer<string> _extraData;

		protected void AddExtraData<V>(string name, V value)
		{
			Util.LazyInitialize<IDataContainer<string>>(ref _extraData, () => new DataContainer<string>())
				.Add<V>(name, value);
		}
		
		#region IDataTransferObject Members

		public IDataContainer<string> ExtraData
		{
			get { return Util.LazyInitialize<IDataContainer<string>>(ref _extraData, () => new DataContainer<string>()); }
			protected set { _extraData = value; }
		}

		#endregion

		#region IDataTransferObjectSPI Members

		void IDataTransferObjectSPI.AddExtraData<V>(string name, V value)
		{
			this.AddExtraData<V>(name, value);
		}

		void IDataTransferObjectSPI.SetExtraData(IDataContainer<string> extraData)
		{
			this.ExtraData = extraData;
		}

		#endregion
				
	}
}
