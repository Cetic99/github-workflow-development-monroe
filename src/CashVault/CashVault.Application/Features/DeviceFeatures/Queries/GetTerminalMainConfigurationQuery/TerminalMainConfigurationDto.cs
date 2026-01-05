
using CashVault.Application.Common.Models;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;

namespace CashVault.Application.Features.DeviceFeatures.Queries;

public class TerminalMainConfigurationDto
{
    public List<MainConfigurationItem> Items { get; set; } = [];
    public List<SelectListItem> TerminalTypes { get; set; } = [];
    public List<SelectListItem> TerminalTypeOptions { get; set; } = [];
    public List<SelectListItem> AvailablePorts { get; set; } = [];
    public Dictionary<string, List<SelectListItem>> AvailableDeviceModels { get; set; }

    public string? DeviceName { get; set; }
    public TerminalMainConfigurationDto(ILocalizer t)
    {
        Items = [];
        AvailablePorts = [];
        TerminalTypeOptions = [
            new SelectListItem()
            {
                Name = t[TerminalType.GamingATM.Code],
                Value = TerminalType.GamingATM.Code
            },
            new SelectListItem()
            {
                Name = t[TerminalType.ParcelLocker.Code],
                Value = TerminalType.ParcelLocker.Code
            },
            new SelectListItem()
            {
                Name = t[TerminalType.Entertainment.Code],
                Value = TerminalType.Entertainment.Code
            },
            new SelectListItem()
            {
                Name = t[TerminalType.BankingAtm.Code],
                Value = TerminalType.BankingAtm.Code
            },
        ];
    }
}