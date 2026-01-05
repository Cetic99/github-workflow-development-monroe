using System.Collections;

namespace CashVault.Application.Common.Models
{
    public class PaginatedResultSet<T> 
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IList<T> Data { get; set; }
    }
}
