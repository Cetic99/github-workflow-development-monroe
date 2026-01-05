using CashVault.Domain.Common;

namespace CashVault.Infrastructure.CMSService
{
    internal class CMSCommands : Enumeration
    {
        public static CMSCommands TicketRedemption = new(1, "ticketRedemption");
        public static CMSCommands TicketRedemptionAck = new(2, "ticketRedemptionAck");
        public static CMSCommands TicketRedemptionNack = new(3, "ticketRedemptionNack");
        public static CMSCommands TicketPrintRequest = new(4, "ticketPrintRequest");
        public static CMSCommands TicketPrintComplete = new(5, "ticketPrintComplete");
        public static CMSCommands TicketPrintFailed = new(6, "ticketPrintFailed");
        public static CMSCommands Transaction = new(9, "transaction");
        public static CMSCommands Event = new(8, "event");

        public static CMSCommands HealthCheck = new(99, "healthCheck");

        private CMSCommands(int id, string name) : base(id, name) { }
    }
}
