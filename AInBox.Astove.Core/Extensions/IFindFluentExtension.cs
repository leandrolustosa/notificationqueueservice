using System;
using System.Collections.Generic;
using System.Linq;
using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Options;
using Autofac;
using AInBox.Astove.Core.Model;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace AInBox.Astove.Core.Extensions
{
    public static class IFindFluentExtensions
    {
        public async static Task<PaginatedMongoList<TModel>> ToPaginatedMongoList<TModel>(this IFindFluent<TModel, TModel> source, int pageIndex, int pageSize)
            where TModel : IModel, new()
        {
            int totalCount = (int)await source.CountAsync();
            int totalPageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
            if ((pageIndex - 1) > totalPageCount)
                pageIndex = totalPageCount;

            var paginatedData = await source.Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();

            return new PaginatedMongoList<TModel>(pageIndex, pageSize, totalCount, paginatedData, source);
        }
    }
}
