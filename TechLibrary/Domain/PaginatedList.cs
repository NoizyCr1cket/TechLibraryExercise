using System;
using System.Collections.Generic;
using System.Linq;

namespace TechLibrary.Domain
{
    public class PaginatedList<T> : List<T>
    {
        public readonly int PageNumber;
        public readonly int PageSize;
        public readonly int TotalCount;
        public readonly int TotalPages;

        /// <param name="source">The queryable source of items that may be added to this list.</param>
        /// <param name="pageNumber">The number of the page of items to add to this list, starting at 1.</param>
        /// <param name="pageSize">The size of each page of items.</param>
        public PaginatedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = source.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            AddRange(source.Skip((PageNumber - 1) * PageSize).Take(PageSize));
        }
    }
}
