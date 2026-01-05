using System;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

namespace CashVault.Domain.Common
{
    public static class BaseHelper
    {
        public static string GetDeviceTypeCode(IBasicHardwareDevice device)
        {
            return device switch
            {
                IBillAcceptor => DeviceType.BillAcceptor.Code.ToLowerInvariant(),
                IBillDispenser => DeviceType.BillDispenser.Code.ToLowerInvariant(),
                ITITOPrinter => DeviceType.TITOPrinter.Code.ToLowerInvariant(),
                IUserCardReader => DeviceType.UserCardReader.Code.ToLowerInvariant(),
                ICabinet => DeviceType.Cabinet.Code.ToLowerInvariant(),
                ICoinAcceptor => DeviceType.CoinAcceptor.Code.ToLowerInvariant(),
                IParcelLocker => DeviceType.ParcelLocker.Code.ToLowerInvariant(),
                _ => "Unknown Device"
            };
        }

        public static double RoundNumber(double number, int decimalPlaces)
        {
            return Math.Round(number, decimalPlaces, MidpointRounding.AwayFromZero);
        }

        public static decimal RoundNumber(decimal number, int decimalPlaces)
        {
            return Math.Round(number, decimalPlaces, MidpointRounding.AwayFromZero);
        }
    }
}
