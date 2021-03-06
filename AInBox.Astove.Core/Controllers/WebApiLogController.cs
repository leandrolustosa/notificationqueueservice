﻿using System;
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
using System.Reflection;
using System.Collections;
using WebApiDoodle.Net.Http.Client.Model;
using Autofac;
using System.Web.Http.Tracing;
using AInBox.Astove.Core.Logging;
using AInBox.Astove.Core.Enums;

namespace AInBox.Astove.Core.Controllers
{
    [Authorize]
    public class WebApiLogController<TLog, TEntity, TModel, TRequestModel, TUpdateRequestModel> : BaseApiController<TEntity, TModel>
        where TLog: class
        where TEntity: class, IEntity, new()
        where TModel: class, IModel, IDto, new()
        where TRequestModel : class, IModel, IBindingModel, new()
        where TUpdateRequestModel : class, IModel, IBindingModel, new()
    {
        private readonly ILog<TLog> _logger;
        private readonly ITraceWriter _tracer;

        public WebApiLogController()
        {   
        }

        public WebApiLogController(IComponentContext container, IEntityService<TEntity> service) 
            : base (container, service)
        {
            this._tracer = GlobalConfiguration.Configuration.Services.GetTraceWriter();
        }

        public WebApiLogController(IComponentContext container, IEntityService<TEntity> service, ILog<TLog> logger)
            : base(container, service)
        {
            this._tracer = GlobalConfiguration.Configuration.Services.GetTraceWriter();
            this._logger = logger;
        }

        [ActionName("DefaultAction")]
        [Stopwatch]
        public virtual IHttpActionResult Get(PaginatedRequestCommand cmd)
        {
            cmd.FilterInternalDictionary = new Dictionary<string, object>();

            this.ListSelecting(cmd);

            this._tracer.Info(this.Request, this.ControllerContext.ControllerDescriptor.ControllerType.FullName, string.Format("Começando Get {0}", cmd.Type));
            this._logger.Info("Info com Logger: {0}", cmd.Type);

            var attr = typeof(TModel).GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();

            if (cmd.Type.Equals(AInBox.Astove.Core.Enums.Utility.GetEnumText(GetTypes.InsertModel), StringComparison.CurrentCultureIgnoreCase))
            {
                try
                {
                    TEntity entity = BaseEntity.ToEntity<TEntity>(new TRequestModel());
                    TRequestModel requestModel = entity.ToModel<TRequestModel, IBindingModel>(this.Container, false, 0);

                    this.ListSelected(requestModel);

                    return Ok(requestModel);
                }
                catch (Exception ex)
                {
                    this._tracer.Info(Request, this.ControllerContext.ControllerDescriptor.ControllerType.FullName, "Tipo da exceção {0} {1}", ex.GetType(), ex.InnerException.GetType());
                    if (ex is ReflectionTypeLoadException || (ex.InnerException != null && ex.InnerException is ReflectionTypeLoadException))
                    {
                        ReflectionTypeLoadException typeLoadException = null;
                        if (ex.InnerException != null && ex.InnerException is ReflectionTypeLoadException)
                            typeLoadException = ex.InnerException as ReflectionTypeLoadException;
                        else
                            typeLoadException = ex as ReflectionTypeLoadException;

                        var loaderExceptins = typeLoadException.LoaderExceptions;
                        foreach (var tlex in loaderExceptins)
                            this._tracer.Error(Request, string.Format("{0} {1}", this.ControllerContext.ControllerDescriptor.ControllerType.FullName, tlex.GetType().FullName), "Erro: {0} {1}", new[] { tlex.Message, tlex.Source });
                    }
                    else
                        this._tracer.Debug(Request, this.ControllerContext.ControllerDescriptor.ControllerType.FullName, ex.InnerException);

                    return InternalServerError(ex);
                }
            }
            else if (cmd.Type.Equals(AInBox.Astove.Core.Enums.Utility.GetEnumText(GetTypes.UpdateModel), StringComparison.CurrentCultureIgnoreCase))
            {
                try
                {
                    string[] includeProperties = null;
                    if (attr != null)
                        includeProperties = attr.Include;

                    TEntity entity = this.Service.GetSingle(cmd.EntityId);
                    if (entity == null)
                        return NotFound();

                    TUpdateRequestModel model = entity.ToModel<TUpdateRequestModel, IModel>(this.Container, false, 0);

                    this.ListSelected(model);

                    return Ok(model);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
            else if (cmd.Type.Equals(AInBox.Astove.Core.Enums.Utility.GetEnumText(GetTypes.PagedGrid), StringComparison.CurrentCultureIgnoreCase))
            {
                this._tracer.Info(this.Request, this.ControllerContext.ControllerDescriptor.ControllerType.FullName, string.Format("Começando Get ", cmd.Type));

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
                    this._tracer.Info(Request, this.ControllerContext.ControllerDescriptor.ControllerType.FullName, "Tipo da exceção {0} {1}", ex.GetType(), (ex.InnerException!=null)? ex.InnerException.GetType() : ex.GetType());
                    if (ex is ReflectionTypeLoadException || (ex.InnerException != null && ex.InnerException is ReflectionTypeLoadException))
                    {
                        ReflectionTypeLoadException typeLoadException = null;
                        if (ex.InnerException != null && ex.InnerException is ReflectionTypeLoadException)
                            typeLoadException = ex.InnerException as ReflectionTypeLoadException;
                        else
                            typeLoadException = ex as ReflectionTypeLoadException;

                        var loaderExceptins = typeLoadException.LoaderExceptions;
                        foreach (var tlex in loaderExceptins)
                            this._tracer.Error(Request, string.Format("{0} {1}", this.ControllerContext.ControllerDescriptor.ControllerType.FullName, tlex.GetType().FullName), "Erro: {0} {1}", new[] { tlex.Message, tlex.Source });
                    }
                    else
                        this._tracer.Debug(Request, this.ControllerContext.ControllerDescriptor.ControllerType.FullName, ex.InnerException);

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
                this._tracer.Info(Request, this.ControllerContext.ControllerDescriptor.ControllerType.FullName, "Tipo da exceção {0} {1}", ex.GetType(), ex.InnerException.GetType());
                if (ex is ReflectionTypeLoadException || (ex.InnerException != null && ex.InnerException is ReflectionTypeLoadException))
                {
                    ReflectionTypeLoadException typeLoadException = null;
                    if (ex.InnerException != null && ex.InnerException is ReflectionTypeLoadException)
                        typeLoadException = ex.InnerException as ReflectionTypeLoadException;
                    else
                        typeLoadException = ex as ReflectionTypeLoadException;

                    var loaderExceptins = typeLoadException.LoaderExceptions;
                    foreach(var tlex in loaderExceptins)
                        this._tracer.Error(Request, string.Format("{0} {1}", this.ControllerContext.ControllerDescriptor.ControllerType.FullName, tlex.GetType().FullName), "Erro: {0} {1}", new[] { tlex.Message, tlex.Source });
                }
                else
                    this._tracer.Debug(Request, this.ControllerContext.ControllerDescriptor.ControllerType.FullName, ex.InnerException);

                return InternalServerError(ex);
            }
        }

        [ActionName("DefaultAction")]
        public virtual IHttpActionResult Get(int id)
        {
            try
            {
                this.Selecting(id);

                var attr = typeof(TModel).GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();
                string[] includeProperties = null;
                if (attr != null)
                    includeProperties = attr.Include;

                TEntity entity = this.Service.GetSingle(id);
                if (entity == null)
                    return NotFound();

                TModel model = this.Service.GetSingle(id, includeProperties).ToModel<TModel, IModel>(this.Container, false, 0);

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

        [ActionName("DefaultAction")]
        [EmptyParameterFilter("request")]
        public virtual IHttpActionResult Post(TRequestModel request)
        {
            _logger.Info("Post ModelState.IsValid {0}", ModelState.IsValid);

            var attr = request.GetType().GetCustomAttributes(true).OfType<ValidationModelAttribute>().FirstOrDefault();
            if (ModelState.IsValid ||
                (!ModelState.IsValid && attr != null && attr.IgnoreProperties != null && ModelState.Keys.Count(k => attr.IgnoreProperties.Any(i => !k.Contains(i))) == 0))
            {
                try
                {
                    this.Inserting(request);

                    TEntity entity = BaseEntity.ToEntity<TEntity>(request);
                    int id = this.Service.Add(entity);

                    this.Inserted(request, entity);

                    return Ok(new HttpResponseResult { Success = true, Id = entity.Id });
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
            else
            {
                return BadRequest(base.ErrorMessage(ModelState));
            }
        }

        [ActionName("ValidateInsert")]
        public virtual IHttpActionResult PostValidateInsert(TRequestModel request)
        {
            if (ModelState.IsValid)
            {
                return Ok(new HttpResponseResult { Success = true });
            }
            else
            {
                return BadRequest(base.ErrorMessage(ModelState));
            }
        }

        [ActionName("DefaultAction")]
        [EmptyParameterFilter("request")]
        public virtual IHttpActionResult Put(int id, TUpdateRequestModel request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    this.Updating(request);

                    TEntity entity = BaseEntity.ToEntity<TEntity>(request);
                    this.Service.Edit(id, entity);

                    this.Updated(request, entity);

                    return Ok(new HttpResponseResult { Success = true, Id = entity.Id });
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
            else
            {
                return BadRequest(base.ErrorMessage(ModelState));
            }
        }

        [ActionName("ValidateUpdate")]
        public virtual IHttpActionResult PutValidateUpdate(int id, TUpdateRequestModel request)
        {
            if (ModelState.IsValid)
            {
                return Ok(new HttpResponseResult { Success = true });
            }
            else
            {
                return BadRequest(base.ErrorMessage(ModelState));
            }
        }

        [ActionName("DefaultAction")]
        public virtual IHttpActionResult Delete(int id)
        {
            try
            {
                this.Deleting(id);

                this.Service.Delete(id);

                this.Deleted(id);

                return Ok(new HttpResponseResult { Success = true });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [NonAction]
        public virtual void Inserting(TRequestModel model)
        {
        }

        [NonAction]
        public virtual void Inserting(TRequestModel model, TEntity entity)
        {
        }

        [NonAction]
        public virtual void Updating(TUpdateRequestModel model)
        {
        }

        [NonAction]
        public virtual void ListSelected(TRequestModel model)
        {
        }

        [NonAction]
        public virtual void ListSelected(TUpdateRequestModel model)
        {
        }
    }
}
