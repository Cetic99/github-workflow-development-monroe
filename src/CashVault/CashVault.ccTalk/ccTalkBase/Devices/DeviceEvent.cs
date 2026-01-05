using System;

namespace CashVault.ccTalk.ccTalkBase.Devices
{
	/// <summary>
	/// Represents a device event for both coin acceptor and bill acceptor
	/// </summary>
	public struct DeviceEvent
	{
		/// <summary>
		/// Constructor for coin acceptor events (original functionality)
		/// </summary>
		/// <param name="coinCode">Coin code (1-255 for valid coins, 0 for errors)</param>
		/// <param name="errorOrRouteCode">Error code or route code</param>
		public DeviceEvent(byte coinCode, byte errorOrRouteCode)
		{
			ResultA = coinCode;
			ResultB = errorOrRouteCode;
			DeviceType = CcTalkDeviceType.CoinAcceptor;
		}

		/// <summary>
		/// Constructor for bill acceptor events
		/// </summary>
		/// <param name="resultA">Bill type (1-255) or status code (0)</param>
		/// <param name="resultB">Action code (0=stacked, 1=escrow) or status detail</param>
		/// <param name="deviceType">Device type identifier</param>
		public DeviceEvent(byte resultA, byte resultB, CcTalkDeviceType deviceType)
		{
			ResultA = resultA;
			ResultB = resultB;
			DeviceType = deviceType;
		}

		/// <summary>
		/// Result A - Bill type (1-255) or coin code, or status indicator (0)
		/// </summary>
		public byte ResultA;

		/// <summary>  
		/// Result B - Action/route code or error/status detail
		/// </summary>
		public byte ResultB;

		/// <summary>
		/// Device type that generated this event
		/// </summary>
		public CcTalkDeviceType DeviceType;

		// Backward compatibility properties for coin acceptor
		/// <summary>
		/// Coin code (backward compatibility)
		/// </summary>
		public byte CoinCode => ResultA;

		/// <summary>
		/// Error or route code (backward compatibility) 
		/// </summary>
		public byte ErrorOrRouteCode => ResultB;

		/// <summary>
		/// Indicates if this is an error event (backward compatibility for coin acceptor)
		/// </summary>
		public bool IsError => ResultA == 0;

		// Bill acceptor specific properties
		/// <summary>
		/// Bill type number (1-255), 0 means status event
		/// </summary>
		public byte BillType => ResultA;

		/// <summary>
		/// Bill action code (0=stacked, 1=escrow) or status detail
		/// </summary>
		public byte ActionCode => ResultB;

		/// <summary>
		/// Indicates if this is a bill credit event (bill validated and stacked)
		/// </summary>
		public bool IsBillCredit => DeviceType == CcTalkDeviceType.BillAcceptor && ResultA >= 1 && ResultA <= 255 && ResultB == 0;

		/// <summary>
		/// Indicates if this is a bill pending credit event (bill in escrow)
		/// </summary>
		public bool IsBillPendingCredit => DeviceType == CcTalkDeviceType.BillAcceptor && ResultA >= 1 && ResultA <= 255 && ResultB == 1;

		/// <summary>
		/// Indicates if this is a bill status event
		/// </summary>
		public bool IsBillStatus => DeviceType == CcTalkDeviceType.BillAcceptor && ResultA == 0;

		/// <summary>
		/// Gets the bill event type based on result codes
		/// </summary>
		public BillEventType GetBillEventType()
		{
			if (DeviceType != CcTalkDeviceType.BillAcceptor)
			{
				return BillEventType.Unknown;
			}

			if (ResultA >= 1 && ResultA <= 255)
			{
				return ResultB switch
				{
					0 => BillEventType.Credit,
					1 => BillEventType.PendingCredit,
					_ => BillEventType.Unknown
				};
			}
			else if (ResultA == 0)
			{
				return ResultB switch
				{
					0 => BillEventType.Status,          // Master inhibit active
					1 => BillEventType.Status,          // Bill returned from escrow
					2 => BillEventType.Reject,          // Invalid bill (validation fail)
					3 => BillEventType.Reject,          // Invalid bill (transport problem)
					4 => BillEventType.Status,          // Inhibited bill (serial)
					5 => BillEventType.Status,          // Inhibited bill (DIP switches)
					6 => BillEventType.FatalError,      // Bill jammed (unsafe mode)
					7 => BillEventType.FatalError,      // Bill jammed in stacker
					8 => BillEventType.FraudAttempt,    // Bill pulled backwards
					9 => BillEventType.FraudAttempt,    // Bill tamper
					10 => BillEventType.Status,         // Stacker OK
					11 => BillEventType.Status,         // Stacker removed
					12 => BillEventType.Status,         // Stacker inserted
					13 => BillEventType.FatalError,     // Stacker faulty
					14 => BillEventType.Status,         // Stacker full
					15 => BillEventType.FatalError,     // Stacker jammed
					16 => BillEventType.FatalError,     // Bill jammed (safe mode)
					17 => BillEventType.FraudAttempt,   // Opto fraud detected
					18 => BillEventType.FraudAttempt,   // String fraud detected
					19 => BillEventType.FatalError,     // Anti-string mechanism faulty
					20 => BillEventType.Status,         // Barcode detected
					21 => BillEventType.Status,         // Unknown bill type stacked
					_ => BillEventType.Unknown
				};
			}

			return BillEventType.Unknown;
		}

		/// <summary>
		/// Gets a description of the bill event
		/// </summary>
		public string GetBillEventDescription()
		{
			if (DeviceType != CcTalkDeviceType.BillAcceptor)
			{
				return "Not a bill acceptor event";
			}

			if (ResultA >= 1 && ResultA <= 255)
			{
				return ResultB switch
				{
					0 => $"Bill type {ResultA} validated and sent to cashbox/stacker",
					1 => $"Bill type {ResultA} validated and held in escrow",
					_ => $"Bill type {ResultA} - unknown action {ResultB}"
				};
			}
			else if (ResultA == 0)
			{
				return ResultB switch
				{
					0 => "Master inhibit active",
					1 => "Bill returned from escrow",
					2 => "Invalid bill (validation fail)",
					3 => "Invalid bill (transport problem)",
					4 => "Inhibited bill (serial)",
					5 => "Inhibited bill (DIP switches)",
					6 => "Bill jammed in transport (unsafe mode)",
					7 => "Bill jammed in stacker",
					8 => "Bill pulled backwards",
					9 => "Bill tamper",
					10 => "Stacker OK",
					11 => "Stacker removed",
					12 => "Stacker inserted",
					13 => "Stacker faulty",
					14 => "Stacker full",
					15 => "Stacker jammed",
					16 => "Bill jammed in transport (safe mode)",
					17 => "Opto fraud detected",
					18 => "String fraud detected",
					19 => "Anti-string mechanism faulty",
					20 => "Barcode detected",
					21 => "Unknown bill type stacked",
					_ => $"Unknown status code {ResultB}"
				};
			}

			return "Unknown event";
		}

		public override string ToString()
		{
			if (DeviceType == CcTalkDeviceType.BillAcceptor)
			{
				return $"Bill Event: {GetBillEventDescription()} (A={ResultA}, B={ResultB})";
			}
			else
			{
				// Coin acceptor format (backward compatibility)
				return $"Coin Event: Code={CoinCode}, ErrorOrRoute={ErrorOrRouteCode}";
			}
		}
	}

	/// <summary>
	/// Device type identifier for ccTalk events
	/// </summary>
	public enum CcTalkDeviceType
	{
		CoinAcceptor,
		BillAcceptor
	}

	/// <summary>
	/// Bill acceptor event types based on ccTalk specification
	/// </summary>
	public enum BillEventType
	{
		Unknown,        // Unknown or invalid event
		Credit,         // Bill accepted - credit the customer
		PendingCredit,  // Bill held in escrow - decide whether to accept
		Reject,         // Bill rejected and returned to customer
		Status,         // Informational only
		FatalError,     // Service callout required
		FraudAttempt    // Fraud detected - possible alarm
	}
}