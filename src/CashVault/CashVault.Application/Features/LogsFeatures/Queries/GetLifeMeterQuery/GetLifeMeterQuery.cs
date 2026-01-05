using CashVault.Application.Interfaces.Persistence;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class GetLifeMeterQueryValidator : AbstractValidator<GetLifeMeterQuery>
    {
        public GetLifeMeterQueryValidator()
        {

        }
    }

    public record GetLifeMeterQuery : IRequest<LifeMeterDto>
    {

    }

    internal sealed class GetLifeMeterQueryHandler : IRequestHandler<GetLifeMeterQuery, LifeMeterDto>
    {
        private readonly ITerminalRepository _db;

        public GetLifeMeterQueryHandler(ITerminalRepository db)
        {
            _db = db;
        }

        public async Task<LifeMeterDto> Handle(GetLifeMeterQuery request, CancellationToken cancellationToken)
        {
            var dto = new LifeMeterDto();

            return await Task.FromResult(dto);
        }
    }
}
