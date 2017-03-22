using System;
using System.Collections.Generic;
using System.Linq;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Attributes;
using AInBox.Astove.Core.Options;
using WebApiDoodle.Net.Http.Client.Model;
using AInBox.Astove.Core.Extensions;
using MongoDB.Driver;

namespace AInBox.Astove.Core.Data
{
    public class PaginatedMongoList<TModel> : List<TModel>
        where TModel : IModel
    {
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPageCount { get; private set; }

        private IFindFluent<TModel, TModel> Source { get; set; }

        public PaginatedMongoList(int pageIndex, int pageSize, int totalCount, IEnumerable<TModel> paginatedData, IFindFluent<TModel, TModel> source)
        {
            try
            {
                AddRange(paginatedData);

                Source = source;
                PageIndex = pageIndex;
                PageSize = pageSize;
                TotalCount = totalCount;
                TotalPageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
            }
            catch
            {
                this.Clear();

                Source = source;
                PageIndex = pageIndex;
                PageSize = pageSize;
                TotalCount = totalCount;
                TotalPageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
            }
        }

        public bool HasPreviousPage
        {
            get { return (PageIndex > 1); }
        }

        public bool HasNextPage
        {
            get { return (PageIndex < TotalPageCount); }
        }

        public PaginatedMongoModel<T> ToPaginatedModel<T>()
            where T : class, IModel, IDto, new()
        {
            var attr = typeof(T).GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();

            return new PaginatedMongoModel<T>
            {
                PagingOptions = new PagingOptions
                {
                    PageIndex = this.PageIndex,
                    PageSize = this.PageSize,
                    PageSizes = (attr != null) ? attr.PageSizes : DataEntityAttribute.DefaultPageSizes,
                    TotalCount = this.TotalCount,
                    TotalPageCount = this.TotalPageCount,
                    HasNextPage = this.HasNextPage,
                    HasPreviousPage = this.HasPreviousPage
                },
                Items = this.Select(o => o.CreateInstanceOf<T>()).ToList()
            };
        }
    }
}