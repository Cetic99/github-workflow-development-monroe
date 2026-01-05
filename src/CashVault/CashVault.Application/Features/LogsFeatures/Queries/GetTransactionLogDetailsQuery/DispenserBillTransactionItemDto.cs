namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class DispenserBillTransactionItemDto
    {
        public int Id { get; set; }
        public int CassetteNumber { get; set; }
        public int BillDenomination { get; set; }
        public int BillCountRequested { get; set; }
        public int BillCountRejected { get; set; }
        public int BillCountDispensed { get; set; }
    }
}