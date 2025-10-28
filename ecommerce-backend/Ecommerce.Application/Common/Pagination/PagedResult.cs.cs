using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Common.Pagination
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; } = [];
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        //public static PagedResult<T> Create(IQueryable<T> source, int page, int pageSize)
        //{
        //    var total = source.Count();
        //    var items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        //    return new PagedResult<T>
        //    {
        //        TotalCount = total,
        //        TotalPages = (int)Math.Ceiling(total / (double)pageSize),
        //        CurrentPage = page,
        //        PageSize = pageSize,
        //        Items = items
        //    };
        //}
    }
}
