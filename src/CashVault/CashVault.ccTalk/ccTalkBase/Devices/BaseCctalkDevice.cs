using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CashVault.ccTalk.ccTalkBase.Checksumms;
using CashVault.ccTalk.ccTalkBase.Devices;
using CashVault.ccTalk.ccTalkBase.Messages;
using CashVault.ccTalk.Common.Exceptions;
using CashVault.DeviceDriver.Common.Helpers;

namespace CashVault.ccTalk.ccTalkBase
{
	/// <summary>
	///  Executes ccTalk commands and parses respond
	/// </summary>
	public abstract partial class BaseCctalkDevice
	{
		public static readonly Byte SourceAddress = 1;

		public Byte Address { get; set; }

		//public ICctalkConnection Connection { get; set; }

		protected readonly Checksum _checksumHandler = new Checksum();

		static readonly Dictionary<Byte, TimeSpan> PollingIntervalUnits
			= new Dictionary<byte, TimeSpan>
			  	{
			  		// 0 - special case
			  		{1, new TimeSpan(0, 0, 0, 0, 1)},//ms 
			  		{2, new TimeSpan(0, 0, 0, 0, 10)},//x10 ms 
			  		{3, new TimeSpan(0, 0, 0, 1, 0)},//seconds 
			  		{4, new TimeSpan(0, 0, 1, 0, 0)},//minutes
			  		{5, new TimeSpan(0, 1, 0, 0, 0)},//hours
			  		{6, new TimeSpan(1, 0, 0, 0, 0)},//days
			  		{7, new TimeSpan(7, 0, 0, 0, 0)},//weeks
			  		{8, new TimeSpan(30, 0, 0, 0, 0)},//months
			  		{9, new TimeSpan(365, 0, 0, 0, 0)},//years
			  	};

		static readonly Dictionary<String, CctalkDeviceTypes> DeviceTypes
			= new Dictionary<String, CctalkDeviceTypes>
			  	{
			  		{"Coin Acceptor", CctalkDeviceTypes.CoinAcceptor},
			  		{"Payout", CctalkDeviceTypes.Payout},
			  		{"Reel", CctalkDeviceTypes.Reel},
			  		{"Bill Validator", CctalkDeviceTypes.BillValidator},
			  		{"Card Reader", CctalkDeviceTypes.CardReader},
			  		{"Changer", CctalkDeviceTypes.Changer},
			  		{"Display", CctalkDeviceTypes.Display},
			  		{"Keypad", CctalkDeviceTypes.Keypad},
			  		{"Dongle", CctalkDeviceTypes.Dongle},
			  		{"Meter", CctalkDeviceTypes.Meter},
			  		{"Bootloader", CctalkDeviceTypes.Bootloader},
			  		{"Power", CctalkDeviceTypes.Power},
			  		{"Printer", CctalkDeviceTypes.Printer},
			  		{"RNG", CctalkDeviceTypes.RNG},
			  		{"Hopper Scale", CctalkDeviceTypes.HopperScale},
			  		{"Coin Feeder", CctalkDeviceTypes.CoinFeeder},
			  		{"Debug", CctalkDeviceTypes.Debug},
			  	};


		protected CctalkMessage CreateMessage(Byte header)
		{
			return new CctalkMessage
					   {
						   DestAddr = Address,
						   SourceAddr = SourceAddress,
						   Header = header,
					   };
		}

		protected bool IsAckMessage(CctalkMessage msg)
		{
			if (msg == null)
			{
				return false;
			}
			else
			{
				return msg.Header == 0
						&& msg.DataLength == 0
						&& msg.DestAddr == SourceAddress
						&& msg.SourceAddr == Address;
			}
		}

		#region Commands

		public bool CmdReset()
		{
			var msg = CreateMessage(1);

			var respond = CcTalkMsgSend(msg, _checksumHandler);
			if (respond == null)
				throw new DeviceCommunicationException("No response received during reset command");

			//TODO: after reset device could not respond for random time. workaround needed. Maybe sleep?
			Thread.Sleep(50); // not wery useful in multithread program

			return IsAckMessage(respond);
		}

		public void CmdSimplePoll()
		{
			var msg = CreateMessage(254);

			var response = CcTalkMsgSend(msg, _checksumHandler);
			if (response == null)
				throw new DeviceCommunicationException("No response received during simple poll");
		}

		public TimeSpan CmdRequestPollingPriority()
		{
			var msg = CreateMessage(249);

			var respond = CcTalkMsgSend(msg, _checksumHandler);
			if (respond == null)
				throw new DeviceCommunicationException("No response received during polling priority request");

			if (respond.DataLength < 2)
				throw new InvalidRespondException(respond);

			var units = respond.Data[0];
			var value = respond.Data[1];

			if (units == 0)
				return TimeSpan.Zero;


			return TimeSpan.FromMilliseconds(PollingIntervalUnits[units].TotalMilliseconds * value);

		}

		public CctalkDeviceStatus CmdRequestStatus()
		{
			var msg = CreateMessage(248);

			var respond = CcTalkMsgSend(msg, _checksumHandler);
			if (respond == null)
				throw new DeviceCommunicationException("No response received during status request");

			if (respond.DataLength < 1)
				throw new InvalidRespondException(respond);

			return (CctalkDeviceStatus)respond.Data[0];
		}

		public String CmdRequestManufacturerId()
		{
			return RequestForStringHelper(246);
		}

		public CctalkDeviceTypes CmdRequestEquipmentCategory()
		{
			var catName = RequestForStringHelper(245);

			CctalkDeviceTypes ret;
			DeviceTypes.TryGetValue(catName, out ret);

			return ret;
		}

		public String CmdRequestProductCode()
		{
			return RequestForStringHelper(244);
		}

		public String CmdRequestSoftwareRevision()
		{
			return RequestForStringHelper(241);
		}

		/// <summary>
		/// Read events from coin acceptor device
		/// </summary>
		public DeviceEventBuffer CmdReadEventBuffer()
		{
			var msg = CreateMessage(229);
			var respond = CcTalkMsgSend(msg, _checksumHandler);

			if (respond == null)
				throw new DeviceCommunicationException("No response received during event buffer read");

			if (respond.DataLength < 11)
				throw new InvalidRespondException(respond);

			var data = respond.Data;
			var events = new[]
							 {
								 new DeviceEvent(data[1], data[2]),
								 new DeviceEvent(data[3], data[4]),
								 new DeviceEvent(data[5], data[6]),
								 new DeviceEvent(data[7], data[8]),
								 new DeviceEvent(data[9], data[10]),
							 }
				;


			var ret = new DeviceEventBuffer
			{
				Counter = respond.Data[0],
				Events = events,
			};

			return ret;
		}

		public Int32 CmdGetSerial()
		{
			var msg = CreateMessage(242);
			var respond = CcTalkMsgSend(msg, _checksumHandler);

			if (respond == null)
				throw new DeviceCommunicationException("No response received during serial number request");

			if (respond.DataLength < 3)
				throw new InvalidRespondException(respond);

			Int32 sn = 0;
			sn += respond.Data[2];
			sn = sn << 8;
			sn += respond.Data[1];
			sn = sn << 8;
			sn += respond.Data[0];

			return sn;
		}


		public void CmdSetMasterInhibitStatus(Boolean isInhibiting)
		{
			var msg = CreateMessage(228);
			msg.Data = new[] {(Byte) (isInhibiting ? 0 : 1)};
			var response = CcTalkMsgSend(msg, _checksumHandler);
			if (response == null)
				throw new DeviceCommunicationException("No response received while setting master inhibit status");
		}

		public Boolean CmdGetMasterInhibitStatus()
		{
			var msg = CreateMessage(227);
			var respond = CcTalkMsgSend(msg, _checksumHandler);
			if (respond == null)
				throw new DeviceCommunicationException("No response received while getting master inhibit status");

			if (respond.DataLength < 1)
				throw new InvalidRespondException(respond);

			bool isInhibiting = (respond.Data[0] & 0x01) == 0; // only last bit significant
			return isInhibiting;
		}

		/*
		 *  Send: [Dir] [2] [2] [231] [Data 1] [Data 2] [Chk]
		 *	Reply: [1] [0] [Dir] [0] [Chk] -> ACK without data
		 *	
		 * [Data 1] = Inhibit byte 1 (LSB), coins 1 to 8
		 * [Data 2] = Inhibit byte 2 (MSB), coins 9 to 16
		 * 
		 * bit 0 (in Data 1): coin 1
		 * bit 15 (in Data 2, equivalent to bit 7 of Data 2): coin 16
		*/
		public void CmdModifyInhibitStatus(int Data1, int Data2)
		{
			var msg = CreateMessage(231);
			msg.Data = new[] { (Byte)Data1, (Byte)Data2 };
			var response = CcTalkMsgSend(msg, _checksumHandler);
			if (response == null)
				throw new DeviceCommunicationException("No response received while modifying inhibit status");
		}

		public byte[] CmdRequestVariableSet()
		{
			byte cmd = 247;
			var msg = CreateMessage(cmd);
			var respond = CcTalkMsgSend(msg, _checksumHandler);
			if (respond == null)
			{
				throw new DeviceCommunicationException($"No response received for Command {cmd} (RequestVariableSet)");
			}
			return respond.Data;
		}

		public FaultCode CmdPerformSelfCheck()
		{
			byte cmd = 232;
			var msg = CreateMessage(cmd);
			var respond = CcTalkMsgSend(msg, _checksumHandler);
			if (respond == null)
			{
				throw new DeviceCommunicationException($"No response received for Command {cmd} (PerformSelfCheck)");
			}
			var fault = FaultCodeTable.CreateFaultCode(respond.Data[0], respond.Data.Length > 1 ? respond.Data[1] : null);
			return fault;
		}
		
		public byte[] CmdRequestInhibitStatus()
		{
			byte cmd = 230;
			var msg = CreateMessage(cmd);
			var respond = CcTalkMsgSend(msg, _checksumHandler);
			if (respond == null)
			{
				throw new DeviceCommunicationException($"No response received for Command {cmd} (RequestInhibitStatus)");
			}

			return respond.Data;
		}


		public class DataStorageAvailability
		{
			public byte MemoryType { get; set; }
			public int ReadBlocks { get; set; }
			public int ReadBytesPerBlock { get; set; }
			public int WriteBlocks { get; set; }
			public int WriteBytesPerBlock { get; set; }
		}

		public DataStorageAvailability CmdRequestDataStorageAvailability()
		{
			byte cmd = 216;
			var msg = CreateMessage(cmd);
			var respond = CcTalkMsgSend(msg, _checksumHandler);

			if (respond == null)
			{
				throw new DeviceCommunicationException("No response received for data storage availability request");
			}

			if (respond.DataLength < 5)
			{
				throw new InvalidRespondException(respond);
			}

			var memoryType = respond.Data[0];
			var readBlocks = respond.Data[1] == 0 ? 256 : respond.Data[1];
			var readBytesPerBlock = respond.Data[2];
			var writeBlocks = respond.Data[3] == 0 ? 256 : respond.Data[3];
			var writeBytesPerBlock = respond.Data[4];

			return new DataStorageAvailability
			{
				MemoryType = memoryType,
				ReadBlocks = readBlocks,
				ReadBytesPerBlock = readBytesPerBlock,
				WriteBlocks = writeBlocks,
				WriteBytesPerBlock = writeBytesPerBlock
			};
		}

		public void CmdRequestOptionFlags()
		{
			byte cmd = 213;
			var msg = CreateMessage(cmd);

			throw new NotImplementedException();
		}

		public void CmdCalculateROMChecksum()
		{
			byte cmd = 197;
			var msg = CreateMessage(cmd);

			throw new NotImplementedException();
		}

		public void CmdRequestBuildCode()
		{
			byte cmd = 192;
			var msg = CreateMessage(cmd);

			throw new NotImplementedException();
		}

		/// <summary>
		/// Read buffered bill events from bill acceptor device
		/// </summary>
		public DeviceEventBuffer CmdRequestBufferedBillEvents()
		{
			byte cmd = 159;
			var msg = CreateMessage(cmd);
			var respond = CcTalkMsgSend(msg, _checksumHandler);

			if (respond == null)
				throw new DeviceCommunicationException("No response received during buffered bill events read");

			if (respond.DataLength < 11)
				throw new InvalidRespondException(respond);

			var data = respond.Data;
			var events = new[]
							 {
								 new DeviceEvent(data[1], data[2], CcTalkDeviceType.BillAcceptor),
								 new DeviceEvent(data[3], data[4], CcTalkDeviceType.BillAcceptor),
								 new DeviceEvent(data[5], data[6], CcTalkDeviceType.BillAcceptor),
								 new DeviceEvent(data[7], data[8], CcTalkDeviceType.BillAcceptor),
								 new DeviceEvent(data[9], data[10], CcTalkDeviceType.BillAcceptor),
							 };

			var ret = new DeviceEventBuffer
			{
				Counter = respond.Data[0],
				Events = events,
			};

			return ret;
		}

		public void CmdRequestBillId()
		{
			byte cmd = 157;
			var msg = CreateMessage(cmd);

			throw new NotImplementedException();
		}

		public void CmdRequestCountryScalingFactor()
		{
			byte cmd = 156;
			var msg = CreateMessage(cmd);

			throw new NotImplementedException();
		}

		public void CmdRouteBill(byte route)
		{
			byte cmd = 154;
			var msg = CreateMessage(cmd);

			throw new NotImplementedException();
		}

		public void CmdModifyBillOperatingMode(bool stacker, bool escrow)
		{
			byte cmd = 153;
			var msg = CreateMessage(cmd);

			// [ mode control mask ]
			// B0 - stacker
			// B1 - escrow
			// 0 = do not use, 1 = use
			byte mod = (byte)((stacker ? 1 : 0) | (escrow ? 2 : 0));
			msg.Data = new[] { mod };

			var response = CcTalkMsgSend(msg, _checksumHandler);
			if (response == null)
			{
				throw new DeviceCommunicationException("No response received while setting bill operating mode");
			}

			if (!IsAckMessage(response))
			{
				throw new InvalidRespondException(response, "Invalid response received while setting bill operating mode");
			}
		}

		public void CmdRequestBillOperatingMode()
		{
			byte cmd = 152;
			var msg = CreateMessage(cmd);

			throw new NotImplementedException();
		}

		public void CmdRequestCurrencyRevision()
		{
			byte cmd = 145;
			var msg = CreateMessage(cmd);

			throw new NotImplementedException();
		}

		public void CmdRequestFirmwareUpgradeCapability()
		{
			byte cmd = 141;
			var msg = CreateMessage(cmd);

			throw new NotImplementedException();
		}

		public void CmdRequestCommsRevision()
		{
			byte cmd = 4;
			var msg = CreateMessage(cmd);

			throw new NotImplementedException();
		}
		#endregion

		String RequestForStringHelper(Byte header)
		{
			var msg = CreateMessage(header);

			var respond = CcTalkMsgSend(msg, _checksumHandler);
			if (respond == null)
				throw new DeviceCommunicationException($"No response received for string helper header {header}");

			if (respond.DataLength < 1)
				throw new InvalidRespondException(respond);

			var ret = ParseAsciiHelper(respond.Data);

			return ret;
		}


		static String ParseAsciiHelper(Byte[] data)
		{
			return Encoding.ASCII.GetString(data);
		}

		public static CctalkMessage ParseRespond(Byte[] source, Int32 offset, Int32 length)
		{
			var ret = ParseMessage(source, offset, length);
			if (ret.Header != 0)
				throw new InvalidRespondFormatException(source, "Invalid respond header. Possible reason: echo is enabled");
			return ret;
		}

		public static CctalkMessage ParseMessage(Byte[] source, Int32 offset, Int32 length)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (source.Length < offset + length)
				throw new ArgumentException("offset or length are invalid");

			if (length < CctalkMessage.MinMessageLength)
				throw new ArgumentException("too small for message", "length");

			//if (header != this.Header)
			//    throw new ArgumentException("invalid message type for this class", "source");

			var dataLen = source[CctalkMessage.PosDataLen + offset];

			if (dataLen + 5 != length)
				throw new ArgumentException("invalid message data lengh", "source");

			var ret = new CctalkMessage
						  {
							  Header = source[CctalkMessage.PosHeader + offset],
							  DestAddr = source[CctalkMessage.PosDestAddr + offset],
							  SourceAddr = source[CctalkMessage.PosSourceAddr + offset]
						  };

			if (dataLen > 0)
			{
				ret.Data = new byte[dataLen];
				Array.Copy(source, CctalkMessage.PosDataStart + offset, ret.Data, 0, dataLen);
			}

			return ret;
		}

		public static Boolean IsRespondComplete(Byte[] respondRawData, Int32 lengthOverride)
		{
			if (lengthOverride <= 4) return false;
			if (lengthOverride > 255) return true;

			var expectedLen = GetExpectedLength(respondRawData);
			return expectedLen == lengthOverride;
		}

		public static Boolean IsRespondComplete(Byte[] respondRawData)
		{
			return IsRespondComplete(respondRawData, respondRawData.Length);
		}

		private static Int32 GetExpectedLength(Byte[] respondRawData)
		{
			if (respondRawData.Length <= CctalkMessage.MinMessageLength)
				throw new InvalidRespondFormatException(respondRawData);

			var dataLen = respondRawData[CctalkMessage.PosDataLen];
			return CctalkMessage.MinMessageLength + dataLen; // 1Src+1Len+1Dest+1Header+Data+1Checksum
		}


	}
}