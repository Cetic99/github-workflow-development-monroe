using System;

namespace CashVault.ccTalk.ccTalkBase.Devices
{
	[Serializable]
	internal class InvalidRespondFormatException : Exception
	{
		public InvalidRespondFormatException(byte[] respondRawData) : this(respondRawData, "Invalid respond")
		{
		}

		public InvalidRespondFormatException(byte[] respondRawData, string message) : base(message)
		{
			InvalidRespondData = respondRawData;
		}

		public byte[] InvalidRespondData { get; private set; }

	}
}