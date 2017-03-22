using System;
using System.Collections.Generic;
using System.Linq;
using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Options;
using Autofac;

namespace AInBox.Astove.Core.Extensions
{
    public static class ICollectionExtensions
    {
        public static PaginatedList<TEntity> ToPaginatedList<TEntity>(this ICollection<TEntity> source, IComponentContext container, AInBox.Astove.Core.Options.Filter filter, KeyValue parentId, int pageIndex, int pageSize)
            where TEntity : IEntity, new()
        {
            FilterOptions options = null;
            FilterCondition[] conditions = null;
            if (filter != null && (filter.Options != null || filter.Conditions != null))
            {
                options = new FilterOptions { Filters = filter.Options.Filters.Where(f => f.Internal == false).ToList() };
                conditions = filter.Conditions.Where(c => c.Internal == false).ToArray();
            }

            int totalCount = source.Count();

            int totalPageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (pageIndex > totalPageCount)
                pageIndex = totalPageCount;

            var collection = source.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PaginatedList<TEntity>(container, options, conditions, parentId, pageIndex, pageSize, totalCount, collection, source);
        }
    }
}
