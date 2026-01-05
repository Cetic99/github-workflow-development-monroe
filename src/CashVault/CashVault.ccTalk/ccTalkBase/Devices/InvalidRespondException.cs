using System;
using CashVault.ccTalk.ccTalkBase.Messages;

namespace CashVault.ccTalk.ccTalkBase.Devices
{
	[Serializable]
	internal class InvalidRespondException : Exception
	{
		public InvalidRespondException(CctalkMessage respond)
			: this(respond, "Invalid respond")
		{
		}

		public InvalidRespondException(CctalkMessage respond, string message)
			: base(message)
		{
			InvalidRespond = respond;
		}

		public CctalkMessage InvalidRespond { get; private set; }

	}
}