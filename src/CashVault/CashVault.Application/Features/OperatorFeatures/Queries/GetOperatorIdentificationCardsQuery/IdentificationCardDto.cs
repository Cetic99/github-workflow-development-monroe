using CashVault.Domain.Aggregates.OperatorAggregate;

namespace CashVault.Application.Features.OperatorFeatures.Queries.GetOperatorIdentificationCardsQuery;

public class IdentificationCardDto
{
    public int Id { get; set; }
    public string? SerialNumber { get; set; }
    public string? IssuedBy { get; set; }
    public DateTime? IssuedAt { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public DateTime? LastStatusChange { get; set; }
    public int OperatorId { get; set; }
    public string Remarks { get; set; }
    public IdentificationCardStatus Status { get; set; }
}