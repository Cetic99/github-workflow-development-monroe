namespace CashVault.Application.Features.AdministrationFeatures.Queries
{
    public class MessagesDto
    {
        public List<MessageItemDto> Items { get; set; }

        public MessagesDto(int page, int perPage, int totalCount)
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