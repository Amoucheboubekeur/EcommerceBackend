using System;
using System.Collections.Generic;


namespace Ecommerce.Domain.DTOs.Orders
{
    public class PagedResultDto<T>
    {
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<T> Data { get; set; } = [];
    }
}
