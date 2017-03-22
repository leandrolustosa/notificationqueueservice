using System;
using System.Collections.Generic;
using System.Linq;
using AInBox.Astove.Core.Filter;

namespace AInBox.Astove.Core.Options
{
    public class PagingOptions
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int[] PageSizes { get; set; }
        public int TotalCount { get; set; }
        public int TotalPageCount { get; set; }

        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}
