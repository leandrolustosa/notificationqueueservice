using System;
using System.Collections.Generic;
using AInBox.Astove.Core.Options;
using WebApiDoodle.Net.Http.Client.Model;

namespace AInBox.Astove.Core.Model
{
    public class PaginatedModel<TModel> : PaginatedDto<TModel>, IPaginatedModel<TModel> 
        where TModel : class, IModel, IDto, new()
    {
        public int ElapsedTime { get; set; }
        public string ControllerName { get; set; }
        public bool EnablePaging { get; set; }
        public bool HasDefaultConditions { get; set; }
        public KeyValue ParentId { get; set; }

        public PagingOptions PagingOptions { get; set; }

        public FilterOptions FilterOptions { get; set; }
        public FilterCondition[] FilterConditions { get; set; }
        public Condition Condition { get; set; }

        public ColumnDefinition[] ColumnDefinitions { get; set; }

        public new IEnumerable<TModel> Items { get; set; }

        public string[] FiltersType { get { return new string[0]; } }
        public string[] Fields { get { return new string[0]; } }
        public string[] Operators { get { return new string[0]; } }
        public string[] Values { get { return new string[0]; } }

        public SortOptions SortOptions { get { return new SortOptions(); } }
    }
}
