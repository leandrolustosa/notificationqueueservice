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
using System.Reflection;
using System.Collections;
using WebApiDoodle.Net.Http.Client.Model;
using Autofac;
using System.Web;
using System.Web.Http.ModelBinding;
using AInBox.Astove.Core.Security;
using System.Web.Http.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using AInBox.Astove.Core.UnitTest;

namespace AInBox.Astove.Core.Controllers
{
    public class BaseApiController<TEntity, TModel> : ApiController, IAstoveApiController
        where TEntity: class, IEntity, new()
        where TModel: class, IModel, IDto, new()
    {
        private readonly IComponentContext _container;
        private readonly IEntityService<TEntity> _service;
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        public IComponentContext Container 
        {
            get { return this._container; }        
        }

        public IEntityService<TEntity> Service 
        {
            get { return this._service; } 
        }

        public BaseApiController()
        {   
        }

        public BaseApiController(IComponentContext container, IEntityService<TEntity> service)
        {
            this._container = container;
            this._service = service;
        }

        public BaseApiController(IComponentContext container, IEntityService<TEntity> service, ApplicationUserManager userManager)
        {
            this._container = container;
            this._service = service;
            UserManager = userManager;
        }

        [NonAction]
        public virtual void Inserted(IBindingModel request, TEntity entity)
        {
        }

        [NonAction]
        public virtual void Updated(IBindingModel request, TEntity entity)
        {
        }

        [NonAction]
        public virtual void Deleting(int id)
        {
        }

        [NonAction]
        public virtual void Deleted(int id)
        {
        }

        [NonAction]
        public virtual void Selecting(int id)
        {
        }

        [NonAction]
        public virtual void Selected(TModel model)
        {
        }

        [NonAction]
        public virtual void ListSelecting(PaginatedRequestCommand command)
        {
        }

        [NonAction]
        public virtual void ListSelected(IPaginatedModel<TModel> model)
        {
        }

        [NonAction]
        public List<string> Errors(ModelStateDictionary modelState)
        {
            var errors = new List<string>();
            foreach (var state in modelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }

            return errors;
        }

        [NonAction]
        public List<string> Errors(Exception ex)
        {
            var errors = new List<string>();
            errors.Add(ex.Message);
            if (ex.InnerException != null)
                errors.Add(ex.InnerException.Message);

            return errors;
        }

        [NonAction]
        public string ErrorMessage(ModelStateDictionary modelState)
        {
            var errors = Errors(modelState);            
            return string.Join(", ", errors.ToArray());
        }

        [NonAction]
        public string ErrorMessage(Exception ex)
        {
            var error = ex.Message;
            if (ex.InnerException != null)
                error = ex.InnerException.Message;

            return error;
        }
        
        [NonAction]
        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        [NonAction]
        protected void SetConditions(TModel model)
        {
            List<string> fields = new List<string>();
            var properties = model.GetType().GetProperties().Where(p => p.PropertyType.Name.Contains("PaginatedModel")).Select(p => p).ToList();
            foreach (var prop in properties)
            {
                var paged = prop.GetValue(model, null);

                var options = paged.GetType().GetProperty("FilterOptions").GetValue(paged, null);

                if (options != null)
                {
                    fields.Add(prop.Name);
                }
            }

            if (fields.Count > 0)
            {
                model.AngularConditions = ConditionTypeBuilder.CreateNewObject(fields.ToArray());

                foreach (var prop in model.AngularConditions.GetType().GetProperties())
                    prop.SetValue(model.AngularConditions, new Condition(), null);
            }
        }

        [NonAction]
        protected void SetChilds(TModel model)
        {
            if (model != null)
            {
                var attr = model.GetType().GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();
                
                string propertyId = string.Format("{0}Id", model.GetType().Name);
                if (attr != null && !string.IsNullOrEmpty(attr.EntityName))
                    propertyId = string.Format("{0}Id", attr.EntityName);

                var properties = model.GetType().GetProperties().Where(p => p.Name.Contains("Child")).Select(p => p);
                foreach (var prop in properties)
                {
                    var obj = Activator.CreateInstance(prop.PropertyType);
                    var miGetModel = obj.GetType().GetMethod("GetModel");
                    obj = miGetModel.Invoke(obj, null);
                    obj.GetType().GetProperty(propertyId).SetValue(obj, model.Id, null);

                    var colDef = prop.GetCustomAttributes(true).OfType<ColumnDefinitionAttribute>().FirstOrDefault();
                    if (colDef != null && colDef.ChildKeys != null && colDef.ChildKeys.Length > 0)
                    {
                        //TODO
                    }
                    prop.SetValue(model, obj, null);
                }
            }
        }

        [NonAction]
        protected void SetViews(object target)
        {
            if (target.GetType().GetInterfaces().Count(i => i.Name.Contains("IEnumerable")) > 0)
            {
                foreach (var obj in (IEnumerable)target)
                {
                    SetPropertyValue(obj);
                }
            }
            else
            {
                SetPropertyValue(target);
            }
        }

        [NonAction]
        private void SetPropertyValue(object target, string[] viewNames, string[] viewProperties, int id)
        {
            if (viewNames != null && viewProperties != null && viewNames.Length == viewProperties.Length)
            {
                for (int i = 0; i < viewNames.Length; i++)
                {
                    var ts = typeof(IEntityService<>);
                    var serviceType = ts.MakeGenericType(new[] { Type.GetType(string.Format(System.Web.Configuration.WebConfigurationManager.AppSettings["DataAssemblyFormat"], viewNames[i])) });

                    var service = this.Container.Resolve(serviceType);

                    var parametersType = new[] { typeof(int) };
                    var mi = service.GetType().GetMethod("GetSingle", parametersType);
                    IEntity entity = (IEntity)mi.Invoke(service, new object[] { id });

                    if (entity != null)
                    {
                        var miToModel = entity.GetType().GetMethod("ToModel");
                        var toModel = miToModel.MakeGenericMethod(new[] { target.GetType().GetProperty(viewProperties[i]).PropertyType });
                        target.GetType().GetProperty(viewProperties[i]).SetValue(target, toModel.Invoke(entity, new object[] { this.Container, true, 0 }), null);
                    }
                }
            }            
        }

        [NonAction]
        private void SetPropertyValue(object obj)
        {
            var attr = obj.GetType().GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();
            SetPropertyValue(obj, attr.ViewNames, attr.ViewProperties, (int)obj.GetType().GetProperty("Id").GetValue(obj, null));

            foreach (var prop in obj.GetType().GetProperties())
            {
                if (prop.PropertyType.GetInterfaces().Count(i => i.Name.Contains("ICollection")) > 0)
                {
                    var entity = prop.GetValue(obj, null);
                    if (entity != null)
                    {
                        foreach (var model in (IEnumerable)entity)
                        {
                            attr = model.GetType().GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();
                            if (attr == null)
                                continue;

                            if (model != null)
                                SetPropertyValue(model, attr.ViewNames, attr.ViewProperties, (int)model.GetType().GetProperty("Id").GetValue(model, null));
                        }
                    }
                }
                else
                {
                    attr = prop.PropertyType.GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();
                    if (attr == null)
                        continue;

                    var model = prop.GetValue(obj, null);
                    if (model != null)
                        SetPropertyValue(model, attr.ViewNames, attr.ViewProperties, (int)prop.PropertyType.GetProperty("Id").GetValue(model, null));
                }
            }
        }
    }
}
