namespace ATTi.Core.Wireup.Metadata
{
	using System;

	/// <summary>
	/// Attribute declaring a wireup command for an assembly.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class WireupAttribute : Attribute
	{
		#region Constructors

		/// <summary>
		/// Creates a new WireupAttribute.
		/// </summary>
		public WireupAttribute()
		{
		}

		/// <summary>
		/// Creates a new WireupAttribute and initializes the command type.
		/// </summary>
		/// <param name="commandType"></param>
		public WireupAttribute(Type commandType)
		{
			Contracts.Require.IsNotNull("commandType", commandType);
			Contracts.Require.IsAssignableFrom<IWireupCommand>("commandType", commandType);
			this.CommandType = commandType;
		}

		#endregion Constructors
		
		/// <summary>
		/// The command type to be invoked during wireup.
		/// </summary>
		public Type CommandType
		{
			get; set;
		}		
	}
}