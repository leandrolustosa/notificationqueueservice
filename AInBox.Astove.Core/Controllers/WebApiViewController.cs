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
    public class WebApiViewController<TEntity, TModel> : BaseApiController<TEntity, TModel>
        where TEntity: class, IEntity, new()
        where TModel: class, IModel, IDto, new()
    {
        public WebApiViewController()
        {   
        }

        public WebApiViewController(IComponentContext container, IEntityService<TEntity> service)
            : base(container, service)
        {
        }

        [ActionName("DefaultAction")]
        [Stopwatch]
        public virtual IHttpActionResult Get(PaginatedRequestCommand cmd)
        {
            cmd.FilterInternalDictionary = new Dictionary<string, object>();

            this.ListSelecting(cmd);

            var attr = typeof(TModel).GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();

            if (cmd.Type.Equals(AInBox.Astove.Core.Enums.Utility.GetEnumText(GetTypes.PagedGrid), StringComparison.CurrentCultureIgnoreCase))
            {
                try
                {
                    var list = this.Service.GetEntities(this.Container, cmd, typeof(TModel), cmd.OrdersBy, cmd.Directions, (attr != null) ? attr.Include : null);

                    var paginatedModel = list.ToPaginatedModel<TModel>(null, null);
                    SetViews(paginatedModel.Items);

                    paginatedModel.ColumnDefinitions = ColumnFactory.GenerateColumnDefinitions<TModel>(ActionEnum.List).ToArray();
                    paginatedModel.Condition = new Condition();

                    this.ListSelected(paginatedModel);

                    return Ok(paginatedModel);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }

            try
            {
                var entities = this.Service.GetEntities(this.Container, cmd, typeof(TModel), cmd.OrdersBy, cmd.Directions, (attr != null) ? attr.Include : null);
                var paged = entities.ToPaginatedModel<TModel>(null, null);
                SetViews(paged.Items);

                paged.ColumnDefinitions = ColumnFactory.GenerateColumnDefinitions<TModel>(ActionEnum.List).ToArray();
                paged.Condition = new Condition();

                this.ListSelected(paged);

                return Ok(paged);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("DefaultAction")]
        public virtual IHttpActionResult Get(int id)
        {
            try
            {
                this.Selecting(id);

                TEntity entity = this.Service.GetSingle(id);
                if (entity == null)
                    return NotFound();

                TModel model = this.Service.GetSingle(id).ToModel<TModel, IModel>(this.Container, false, 0);
                SetViews(model);
                SetConditions(model);
                SetChilds(model);

                this.Selected(model);

                return Ok(model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
