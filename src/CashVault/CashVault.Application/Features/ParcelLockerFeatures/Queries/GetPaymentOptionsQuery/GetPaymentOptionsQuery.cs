using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using MediatR;

namespace CashVault.Application.Features.ParcelLockerFeatures.Queries;

public record GetPaymentOptionsQuery : IRequest<List<PaymentOptionDto>>
{ }

internal sealed class GetPaymentOptionsQueryHandler(ITerminalRepository db)
    : IRequestHandler<GetPaymentOptionsQuery, List<PaymentOptionDto>>
{
    public ITerminalRepository _db = db;

    public async Task<List<PaymentOptionDto>> Handle(GetPaymentOptionsQuery request, CancellationToken cancellationToken)
    {
        SupportedPaymentOptionsConfiguration? paymentOptionsConfiguration =
            await _db.GetSupportedPaymentOptionsConfigurationAsync();

        if (paymentOptionsConfiguration is null)
            return [];

        return paymentOptionsConfiguration.PaymentOptions.ConvertAll(x => new PaymentOptionDto() { Code = x.Code });
    }
}
