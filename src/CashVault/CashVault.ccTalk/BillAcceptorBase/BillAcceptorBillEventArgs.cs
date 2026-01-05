using System;
using CashVault.ccTalk.ccTalkBase.Devices;

namespace CashVault.ccTalk.BillAcceptorBase
{
	/// <summary>
	/// Data for BillAcceptor's event. Contains various info about accepted bill
	/// </summary>
	public class BillAcceptorBillEventArgs : EventArgs
	{
		/// <summary>
		/// Name of bill as was set in BillAcceptor constructor.
		/// </summary>
		public String BillName { get; private set; }
		
		/// <summary>
		/// Value of the accepted bill
		/// </summary>
		public Decimal BillValue { get; private set; }

		/// <summary>
		/// Device's code for bill (bill type 1-255)
		/// </summary>
		public Byte BillCode { get; private set; }

		/// <summary>
		/// Device's route/action code (0=stacked, 1=escrow)
		/// </summary>
		public Byte ActionCode { get; private set; }

		/// <summary>
		/// The underlying bill event
		/// </summary>
		public DeviceEvent BillEvent { get; private set; }

		/// <summary>
		/// Type of bill event (Credit, PendingCredit, etc.)
		/// </summary>
		public BillEventType EventType { get; private set; }

		/// <summary>
		/// Indicates if bill was stacked (accepted)
		/// </summary>
		public bool IsStacked => ActionCode == 0;

		/// <summary>
		/// Indicates if bill is in escrow (pending decision)
		/// </summary>
		public bool IsInEscrow => ActionCode == 1;

		/// <summary>
		/// Creates instance of BillAcceptorBillEventArgs from bill information
		/// </summary>
		/// <param name="billName">Name of the bill</param>
		/// <param name="billValue">Value of the bill</param>
		/// <param name="billCode">Bill type code</param>
		/// <param name="actionCode">Action code (0=stacked, 1=escrow)</param>
		public BillAcceptorBillEventArgs(String billName, decimal billValue, Byte billCode, Byte actionCode)
		{
			BillName = billName;
			BillValue = billValue;
			BillCode = billCode;
			ActionCode = actionCode;
			BillEvent = new DeviceEvent(billCode, actionCode, CcTalkDeviceType.BillAcceptor);
			EventType = BillEvent.GetBillEventType();
		}

		/// <summary>
		/// Creates instance of BillAcceptorBillEventArgs from DeviceEvent
		/// </summary>
		/// <param name="billEvent">The bill event</param>
		/// <param name="billName">Name of the bill (if known)</param>
		/// <param name="billValue">Value of the bill (if known)</param>
		public BillAcceptorBillEventArgs(DeviceEvent billEvent, string? billName = null, decimal billValue = 0)
		{
			BillEvent = billEvent;
			BillCode = billEvent.BillType;
			ActionCode = billEvent.ActionCode;
			EventType = billEvent.GetBillEventType();
			BillName = billName ?? $"Bill Type {BillCode}";
			BillValue = billValue;
		}

		/// <summary>
		/// Route path (backward compatibility)
		/// </summary>
		public Byte RoutePath => ActionCode;
	}
}