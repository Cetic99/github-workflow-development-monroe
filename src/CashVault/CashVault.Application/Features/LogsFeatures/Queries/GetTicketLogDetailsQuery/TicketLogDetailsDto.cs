using CashVault.Domain.Aggregates.TicketAggregate;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using System.Reflection.Emit;
using CashVault.Domain.ValueObjects;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class TicketLogDetailsDto
    {
        public int Id { get; set; }
        public decimal? Amount { get; set; }
        public string? Barcode { get; set; }
        public string? Type { get; set; }
        public string? Number { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public DateTime? PrintingRequestedAt { get; set; }
        public DateTime? PrintingCompletedAt { get; set; }
        public Currency? Currency { get; set; }

        public int? DaysValid { get; set; }
        public bool? IsLocal { get; set; }
        public bool? IsPrinted { get; set; }
        public bool? IsStacked { get; set; }
        public bool? IsExpired { get; set; }
        public bool? IsValid { get; set; }
        public bool? CanBeRedeemed { get; set; }
    }
}