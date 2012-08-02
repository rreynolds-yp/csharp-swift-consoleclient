using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATTi.Core.Data
{
	[Flags]
	public enum TransactionEscalation
	{
		/// <summary>
		/// Indicates no escalation is necessary.
		/// </summary>
		None = 0,
		/// <summary>
		/// Indicates that a resource uses only local resources.
		/// </summary>
		Local = 1,
		/// <summary>
		/// Indicates that a resource requires escalation to a distributed transaction.
		/// </summary>
		Escalate = 2,
		/// <summary>
		/// Indicates that the resource delegates to another resource that requires escalation.
		/// </summary>
		EscalateForDelegation = 6
	}


	public interface ITransactionResource
	{
		TransactionEscalation DefaultEscalation { get; }
		TransactionEscalation CalculateTotalEscalation();
	}
}
