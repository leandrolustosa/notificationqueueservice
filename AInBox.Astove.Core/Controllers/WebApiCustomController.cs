using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AInBox.Astove.Core.Filter;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Options;
using AInBox.Astove.Core.Attributes;
using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.List;
using AInBox.Astove.Core.Options.EnumDomainValues;
using System.Collections;
using WebApiDoodle.Net.Http.Client.Model;
using Autofac;
using AInBox.Astove.Core.Enums;

namespace AInBox.Astove.Core.Controllers
{
    [Authorize]
    public class WebApiCustomController<TEntity, TModel> : BaseApiController<TEntity, TModel>
        where TEntity : class, IEntity, new()
        where TModel : class, IModel, IDto, new()
    {
        public WebApiCustomController()
        {
        }

        public WebApiCustomController(IComponentContext container, IEntityService<TEntity> service)
            : base(container, service)
        {
        }
    }
}
