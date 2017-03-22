using System;
using System.Collections.Generic;
using AInBox.Astove.Core.Options;
using WebApiDoodle.Net.Http.Client.Model;

namespace AInBox.Astove.Core.Model
{
    public class PaginatedMongoModel<TModel> : PaginatedDto<TModel>, IPaginatedMongoModel<TModel> 
        where TModel : class, IModel, IDto, new()
    {
        public int ElapsedTime { get; set; }
        
        public PagingOptions PagingOptions { get; set; }

        public new IEnumerable<TModel> Items { get; set; }
    }
}
