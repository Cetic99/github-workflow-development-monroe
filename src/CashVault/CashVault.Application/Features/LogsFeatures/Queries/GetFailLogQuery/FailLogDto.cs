namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class FailLogDto
    {
        public List<LogItemDto> Items { get; set; }

        public FailLogDto(int page, int perPage, int totalCount)
        {
            Items = [];
            Page = page;
            PageSize = perPage;
            TotalCount = totalCount;
        }

        public int Page { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
    }
}