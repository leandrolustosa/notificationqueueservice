using System;
using System.Collections.Generic;
using AInBox.Astove.Core.Options;

namespace AInBox.Astove.Core.Model
{
    public interface IPaginatedMongoModel<out TModel> where TModel : IModel
    {
        int ElapsedTime { get; set; }
        PagingOptions PagingOptions { get; set; }
        IEnumerable<TModel> Items { get; }
    }
}
