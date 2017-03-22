using System;
using System.Collections.Generic;
using AInBox.Astove.Core.Options;

namespace AInBox.Astove.Core.Model
{
    public interface IPaginatedModel<out TModel> where TModel : IModel
    {
        int ElapsedTime { get; set; }
        string ControllerName { get; set; }
        bool EnablePaging { get; set; }
        bool HasDefaultConditions { get; set; }
        KeyValue ParentId { get; set; }

        PagingOptions PagingOptions { get; set; }

        FilterOptions FilterOptions { get; set; }
        FilterCondition[] FilterConditions { get; set; }
        Condition Condition { get; set; }

        ColumnDefinition[] ColumnDefinitions { get; set; }
        
        IEnumerable<TModel> Items { get; }
    }
}
