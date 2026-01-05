using CashVault.Application.Common.Models;
using CashVault.Domain.Aggregates.TransactionAggregate;
using CashVault.Domain.TransactionAggregate;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class TransactionLogsDto
    {
        public List<TransactionDto> Transactions { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public List<TransactionType> TransactionTypes { get; set; }
        public List<TransactionStatus> TransactionStatuses { get; set; }
        public List<SelectListItem> TransactionKindTypes { get; set; }

        public TransactionLogsDto()
        {
            Transactions = new List<TransactionDto>();
            TransactionKindTypes =
            [
                new SelectListItem
                {
                    Name = (nameof(TicketTransaction)),
                    Value = (nameof(TicketTransaction))
                },
                new SelectListItem
                {
                    Name = (nameof(DispenserBillTransaction)),
                    Value = (nameof(DispenserBillTransaction))
                },
                new SelectListItem
                {
                    Name = (nameof(AcceptorBillTransaction)),
                    Value = (nameof(AcceptorBillTransaction))
                },
            ];
        }
    }
}