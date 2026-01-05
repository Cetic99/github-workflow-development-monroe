﻿using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class GetDeviceInfoQueryValidator : AbstractValidator<GetDeviceInfoQuery>
    {
        public GetDeviceInfoQueryValidator()
        {
            RuleFor(x => x.Type).NotNull().NotEmpty().WithMessage("Type is required.");
        }
    }

    public record GetDeviceInfoQuery : IRequest<DeviceInfoDto>
    {
        public string Type { get; set; } = null!;
    }

    internal sealed class GetDeviceInfoQueryHandler : IRequestHandler<GetDeviceInfoQuery, DeviceInfoDto>
    {
        private readonly ITerminal _terminal;

        public GetDeviceInfoQueryHandler(ITerminal terminal)
        {
            _terminal = terminal;
        }

        public async Task<DeviceInfoDto> Handle(GetDeviceInfoQuery query, CancellationToken cancellationToken)
        {
            var dto = new DeviceInfoDto();

            if (DeviceType.TITOPrinter.Code.Equals(query.Type, StringComparison.InvariantCultureIgnoreCase) && _terminal.TITOPrinter != null)
            {
                dto.Info = _terminal.TITOPrinter.GetAdditionalDeviceInfo();
            }
            else if (DeviceType.BillAcceptor.Code.Equals(query.Type, StringComparison.InvariantCultureIgnoreCase) && _terminal.BillAcceptor != null)
            {
                dto.Info = _terminal.BillAcceptor.GetAdditionalDeviceInfo();
            }
            else if (DeviceType.BillDispenser.Code.Equals(query.Type, StringComparison.InvariantCultureIgnoreCase) && _terminal.BillDispenser != null)
            {
                dto.Info = _terminal.BillDispenser.GetAdditionalDeviceInfo();
            }
            else if (DeviceType.CoinAcceptor.Code.Equals(query.Type, StringComparison.InvariantCultureIgnoreCase) && _terminal.CoinAcceptor != null)
            {
                dto.Info = _terminal.CoinAcceptor.GetAdditionalDeviceInfo();
            }
            else if (DeviceType.Cabinet.Code.Equals(query.Type, StringComparison.InvariantCultureIgnoreCase) && _terminal.Cabinet != null)
            {
                dto.Info = _terminal.Cabinet.GetAdditionalDeviceInfo();
            }
            else if (DeviceType.UserCardReader.Code.Equals(query.Type, StringComparison.InvariantCultureIgnoreCase) && _terminal.UserCardReader != null)
            {
                dto.Info = _terminal.UserCardReader.GetAdditionalDeviceInfo();
            }

            return await Task.FromResult(dto);
        }
    }
}
