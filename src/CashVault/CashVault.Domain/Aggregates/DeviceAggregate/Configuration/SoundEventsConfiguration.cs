using System.Collections.Generic;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Configuration;

public class SoundEventsConfiguration
{
    public List<SoundEvent> SoundEvents { get; set; }

    public SoundEventsConfiguration()
    {
        SoundEvents = new List<SoundEvent>()
        {
            new SoundEvent() { Id = 1, Name = "Card inserted", SoundTypeCode = null  },
            new SoundEvent() { Id = 2, Name = "Acceptor stacker inserted", SoundTypeCode = null  },
            new SoundEvent() { Id = 3, Name = "Acceptor stacker removed", SoundTypeCode = null  },
            new SoundEvent() { Id = 4, Name = "Call operator", SoundTypeCode = null  },
            new SoundEvent() { Id = 5, Name = "Customer money ready", SoundTypeCode = null  },
        };
    }
}

public class SoundEvent
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? SoundTypeCode { get; set; }
}