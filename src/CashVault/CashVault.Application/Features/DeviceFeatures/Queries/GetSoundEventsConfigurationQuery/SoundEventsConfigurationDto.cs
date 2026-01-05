namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class SoundEventsConfigurationDto
    {
        public List<SoundEventDto> SoundEvents { get; set; }
        public List<SoundType> SoundTypes { get; set; }

        public SoundEventsConfigurationDto()
        {
            SoundTypes = new List<SoundType>()
            {
                new SoundType() { Code = "ding", Name = "ding" },
                new SoundType() { Code = "error", Name = "error" },
                new SoundType() { Code = "pop", Name = "pop" },
                new SoundType() { Code = "success", Name = "success" },
                new SoundType() { Code = "splash", Name = "splash" },
            };
        }
    }

    public class SoundType
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class SoundEventDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? SoundTypeCode { get; set; }
    }
}