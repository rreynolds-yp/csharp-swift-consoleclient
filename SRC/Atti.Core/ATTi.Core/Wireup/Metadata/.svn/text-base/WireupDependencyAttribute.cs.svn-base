namespace ATTi.Core.Wireup.Metadata
{
	using System;

	/// <summary>
	/// Attribute declaring a wireup dependance on another type (a "reliant" type).
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Interface)]
	public sealed class WireupDependencyAttribute : Attribute
	{
		#region Constructors

		/// <summary>
		/// Creates a new WireupDependency.
		/// </summary>
		public WireupDependencyAttribute()
		{
		}

		/// <summary>
		/// Createas a new WireupDependency and initializes it with a reliant type.
		/// </summary>
		/// <param name="reliantType">The type upon which the attribute target is reliant</param>
		public WireupDependencyAttribute(Type reliantType)
		{
			Contracts.Require.IsNotNull("reliantType", reliantType);
			this.ReliantType = reliantType;
		}

		#endregion Constructors
		
		/// <summary>
		/// Indicates the type upon which the attribute target is reliant.
		/// </summary>
		public Type ReliantType
		{
			get; set;
		}		
	}
}