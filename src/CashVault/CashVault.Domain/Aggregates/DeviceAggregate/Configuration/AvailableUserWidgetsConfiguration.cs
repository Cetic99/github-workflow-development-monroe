using System;
using System.Collections.Generic;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Configuration
{
    public class AvailableUserWidget
    {
        public string Code { get; set; } = string.Empty;
    }

    public class AvailableUserWidgetsConfiguration
    {
        /// <summary>
        /// Represented as list of objects so they could be extended in the future.
        /// </summary>
        public List<AvailableUserWidget> AvailableWidgets { get; set; } = [];

        public AvailableUserWidgetsConfiguration() { }

        public void Initialize()
        {
            if (AvailableWidgets is null || AvailableWidgets.Count == 0)
                AvailableWidgets = [new AvailableUserWidget()
                {
                    Code = "TicketPrinterComponent"
                }, new AvailableUserWidget()
                {
                    Code = "MoneyWithdrawComponent"
                }];
        }

        public void UpdateAvailableWidgets(List<string>? availableWidgets) // maybe here should be objects of AvailableUserWidget class
        {
            ArgumentNullException.ThrowIfNull(availableWidgets, nameof(availableWidgets));

            // at least one available widget should be available
            if (availableWidgets.Count == 0)
                throw new ArgumentException(nameof(availableWidgets));

            AvailableWidgets = availableWidgets.ConvertAll(w => new AvailableUserWidget() { Code = w });
        }
    }
}
