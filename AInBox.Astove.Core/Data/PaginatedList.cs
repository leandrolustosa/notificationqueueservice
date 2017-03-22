using System;
using System.Collections.Generic;
using System.Linq;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Attributes;
using AInBox.Astove.Core.Options;
using WebApiDoodle.Net.Http.Client.Model;
using Autofac;
using AInBox.Astove.Core.Filter;

namespace AInBox.Astove.Core.Data
{
    public class PaginatedList<TEntity> : List<TEntity>
        where TEntity: IEntity
    {
        public IComponentContext Container { get; set; }

        public FilterOptions FilterOptions { get; set; }
        public FilterCondition[] FilterConditions { get; set; }

        public KeyValue ParentId { get; set; }

        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPageCount { get; private set; }

        private IEnumerable<TEntity> Source { get; set; }

        public PaginatedList(IComponentContext container, FilterOptions options, FilterCondition[] conditions, KeyValue parentId, int pageIndex, int pageSize, int totalCount, IEnumerable<TEntity> paginatedData, IEnumerable<TEntity> source)
        {
            try
            {
                AddRange(paginatedData);

                Source = source;
                Container = container;
                ParentId = parentId;
                FilterOptions = options;
                FilterConditions = conditions;
                PageIndex = pageIndex;
                PageSize = pageSize;
                TotalCount = totalCount;
                TotalPageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
            }
            catch
            {
                this.Clear();

                Source = source;
                Container = container;
                ParentId = parentId;
                FilterOptions = options;
                FilterConditions = conditions;
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

        public PaginatedModel<TModel> ToPaginatedModel<TModel>(string whereClause, string orderBy) 
            where TModel : class, IModel, IDto, new()
        {
            string predicate = (!string.IsNullOrEmpty(whereClause)) ? whereClause : FilterFactory.GetWhereClause(typeof(TModel), this.FilterConditions);
            object[] parameters = (!string.IsNullOrEmpty(whereClause)) ? null : FilterFactory.GetParameters(typeof(TModel), this.FilterConditions);

            if (!string.IsNullOrEmpty(predicate))
            {
                this.Clear();
                var query = this.Source.AsQueryable().Where(predicate, parameters);
                if (!string.IsNullOrEmpty(orderBy))
                    query = query.OrderBy(orderBy, null);

                this.TotalCount = query.Count();
                this.TotalPageCount = (int)Math.Ceiling(this.TotalCount / (double)this.PageSize);
                this.AddRange(query.AsEnumerable().Skip((this.PageIndex - 1) * this.PageSize).Take(this.PageSize).ToArray());
            }

            var attr = typeof(TModel).GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();

            return new PaginatedModel<TModel>
            {
                ControllerName = (attr != null && !string.IsNullOrEmpty(attr.BaseUriTemplate)) ? attr.BaseUriTemplate.Replace("api/v1/", "") : string.Empty,
                ParentId = this.ParentId,
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
                EnablePaging = true,
                Items = this.Select(c => c.ToModel<TModel, IModel>(this.Container, false, 0)),
                FilterOptions = this.FilterOptions,
                FilterConditions = this.FilterConditions
            };
        }
    }
}
