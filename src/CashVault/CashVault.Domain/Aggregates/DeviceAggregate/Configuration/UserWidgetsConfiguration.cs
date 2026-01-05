using System;
using System.Collections.Generic;
using System.Linq;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Configuration
{
    public class UserWidget
    {
        public Guid Uuid { get; set; }
        public string Code { get; set; } = string.Empty;
        public UserWidgetSize Size { get; set; } = UserWidgetSize.DefaultWidgetSize;
        public int DisplaySequence { get; set; } = 9999;
        public bool Enabled { get; set; } = true;

        public static UserWidget Default(string code)
        {
            return new UserWidget()
            {
                Uuid = Guid.NewGuid(),
                Code = code,
                Enabled = false,
                DisplaySequence = 9999,
                Size = UserWidgetSize.DefaultWidgetSize
            };
        }
    }

    public class UserWidgetsConfiguration
    {
        public List<UserWidget> Widgets { get; set; } = [];

        public UserWidgetsConfiguration()
        { }

        public void Initialize()
        {
            if (Widgets is null || Widgets.Count == 0)
                Widgets = [new UserWidget()
                {
                    Uuid = Guid.NewGuid(),
                    Code = "TicketPrinterComponent",
                    DisplaySequence = 1
                }, new UserWidget ()
                {
                    Uuid = Guid.NewGuid(),
                    Code = "MoneyWithdrawComponent",
                    DisplaySequence = 2
                },new UserWidget ()
                {
                    Uuid = Guid.NewGuid(),
                    Code = "BetboxTicketComponent",
                    DisplaySequence = 2
                }];
        }

        public void SetWidgets(List<UserWidget>? widgets)
        {
            ArgumentNullException.ThrowIfNull(widgets, nameof(widgets));

            // at least one enabled widget should be available
            if (widgets.Count == 0 || !widgets.Any(w => w.Enabled))
                throw new ArgumentException(nameof(widgets));

            Widgets = widgets;
        }

        public void AddWidget(UserWidget? widget)
        {
            ArgumentNullException.ThrowIfNull(widget, nameof(widget));

            if (Widgets.Any(w => w.Code.Equals(widget.Code)))
                throw new InvalidOperationException(nameof(widget.Code));

            Widgets.Add(widget);
        }

        public void RemoveWidget(string? code)
        {
            ArgumentNullException.ThrowIfNull(code, nameof(code));

            if (!Widgets.Any(w => w.Code.Equals(code)))
                throw new InvalidOperationException(nameof(code));

            if (Widgets.Count == 1)
                throw new InvalidOperationException("At least one widget should be available");

            Widgets.RemoveAll(w => w.Code.Equals(code));
        }
    }
}
