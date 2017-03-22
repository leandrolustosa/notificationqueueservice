using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Options;
using Autofac;
using AInBox.Astove.Core.Filter;
using System.Threading.Tasks;
using System.Data.Entity;

namespace AInBox.Astove.Core.Extensions
{
    public static class IQueryableExtensions
    {
        public static PaginatedList<T> ToPaginatedList<T>(this IQueryable<T> query, IComponentContext container,  AInBox.Astove.Core.Options.Filter filter, KeyValue parentId, int pageIndex, int pageSize)
            where T: IEntity, new()
        {
            FilterOptions options = null;
            FilterCondition[] conditions = null;
            if (filter != null && (filter.Options != null || filter.Conditions != null))
            {
                options = new FilterOptions { Filters = filter.Options.Filters.Where(f => f.Internal == false).ToList() };
                conditions = filter.Conditions.Where(c => c.Internal == false).ToArray();
            }

            int totalCount = query.Count();
            int totalPageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
            if ((pageIndex - 1) > totalPageCount)
                pageIndex = totalPageCount;

            var collection = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PaginatedList<T>(container, options, conditions, parentId, pageIndex, pageSize, totalCount, collection, query);
        }

        public async static Task<PaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> query, IComponentContext container, AInBox.Astove.Core.Options.Filter filter, KeyValue parentId, int pageIndex, int pageSize)
            where T : IEntity, new()
        {
            FilterOptions options = null;
            FilterCondition[] conditions = null;
            if (filter != null && (filter.Options != null || filter.Conditions != null))
            {
                options = new FilterOptions { Filters = filter.Options.Filters.Where(f => f.Internal == false).ToList() };
                conditions = filter.Conditions.Where(c => c.Internal == false).ToArray();
            }

            int totalCount = await query.CountAsync();
            int totalPageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
            if ((pageIndex - 1) > totalPageCount)
                pageIndex = totalPageCount;

            var collection = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PaginatedList<T>(container, options, conditions, parentId, pageIndex, pageSize, totalCount, collection, query);
        }

        public static HashSet<T> ToHashSet<T>(this IQueryable<T> query)
        {
            return new HashSet<T>(query);
        }
    }
}
