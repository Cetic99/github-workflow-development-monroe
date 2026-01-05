using CashVault.Application.Interfaces.Persistence;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class GetPayoutRulesQueryValidator : AbstractValidator<GetPayoutRulesQuery>
    {
        public GetPayoutRulesQueryValidator()
        {

        }
    }

    public record GetPayoutRulesQuery : IRequest<PayoutRulesDto>
    {

    }

    internal sealed class GetPayoutRulesQueryHandler : IRequestHandler<GetPayoutRulesQuery, PayoutRulesDto>
    {
        private readonly ITerminalRepository _db;

        public GetPayoutRulesQueryHandler(ITerminalRepository db)
        {
            _db = db;
        }

        public async Task<PayoutRulesDto> Handle(GetPayoutRulesQuery request, CancellationToken cancellationToken)
        {
            var dto = new PayoutRulesDto();
            var config = await _db.GetPayoutRulesConfigurationAsync();

            dto.Tickets = new PayoutRuleDto() { Min = config.Tickets.Min, Max = config.Tickets.Max };
            dto.Bills = new PayoutRuleDto() { Min = config.Bills.Min, Max = config.Bills.Max };
            dto.Coins = new PayoutRuleDto() { Min = config.Coins.Min, Max = config.Coins.Max };

            return await Task.FromResult(dto);
        }
    }
}
