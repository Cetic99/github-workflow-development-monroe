using System;
using CashVault.ccTalk.ccTalkBase.Devices;

namespace CashVault.ccTalk.BillAcceptorBase
{
	/// <summary>
	/// Event arguments for bill acceptor errors and issues.
	/// Uses ccTalk specification: DeviceEvent (bill events) and FaultCode for error reporting.
	/// </summary>
	public class BillAcceptorErrorEventArgs : EventArgs
	{
		/// <summary>
		/// Human-readable error message
		/// </summary>
		public string ErrorMessage { get; private set; }
		
		/// <summary>
		/// Associated bill event (if applicable)
		/// </summary>
		public DeviceEvent? BillEvent { get; private set; }
		
		/// <summary>
		/// Associated fault code (if applicable)
		/// </summary>
		public FaultCode? FaultCode { get; private set; }

		/// <summary>
		/// Creates error event args for general errors
		/// </summary>
		/// <param name="errorMessage">Error message</param>
		public BillAcceptorErrorEventArgs(string errorMessage)
		{
			ErrorMessage = errorMessage;
			BillEvent = null;
			FaultCode = null;
		}

		/// <summary>
		/// Creates error event args for bill event-related errors
		/// </summary>
		/// <param name="billEvent">The bill event that caused the error</param>
		/// <param name="errorMessage">Additional error message (optional)</param>
		public BillAcceptorErrorEventArgs(DeviceEvent billEvent, string? errorMessage = null)
		{
			BillEvent = billEvent;
			ErrorMessage = errorMessage ?? billEvent.GetBillEventDescription();
			FaultCode = null;
		}

		/// <summary>
		/// Creates error event args for fault code-related errors
		/// </summary>
		/// <param name="faultCode">The fault code</param>
		/// <param name="errorMessage">Additional error message (optional)</param>
		public BillAcceptorErrorEventArgs(FaultCode faultCode, string? errorMessage = null)
		{
			FaultCode = faultCode;
			ErrorMessage = errorMessage ?? faultCode.ToString();
			BillEvent = null;
		}

		/// <summary>
		/// Indicates if this error is critical and requires attention
		/// </summary>
		public bool IsCritical => 
			(BillEvent != null && (BillEvent.Value.GetBillEventType() == BillEventType.FatalError || 
			                      BillEvent.Value.GetBillEventType() == BillEventType.FraudAttempt)) ||
			(FaultCode != null && FaultCodeTable.GetSeverity(FaultCode.Code) == FaultSeverity.Critical) ||
			// ErrorMessage.Contains("Communication error") || 
			ErrorMessage.Contains("Device reset");
	}
}