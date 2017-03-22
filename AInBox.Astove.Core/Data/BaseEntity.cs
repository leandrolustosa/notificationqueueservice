using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Attributes;
using AInBox.Astove.Core.Extensions;
using System.Runtime.CompilerServices;
using System.Reflection;
using AInBox.Astove.Core.Filter;
using Autofac;
using AInBox.Astove.Core.Options;
using System.Text.RegularExpressions;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace AInBox.Astove.Core.Data
{
    public class BaseEntity : IEntity
    {
        public virtual int Id { get; set; }
        [NotMapped]
        public string EntityType { get { return this.GetType().Name; } }

        public BaseEntity()
        {   
        }

        public TModel ToModel<TModel, TInterfaceConstraint>(IComponentContext container, bool lazyLoading, int level) where TModel : TInterfaceConstraint, new()
        {
            TModel model = new TModel();

            Type t = typeof(TModel);
            Type c = this.GetType();

            try
            {
                foreach (var propInfo in t.GetProperties())
                {
                    var attr = propInfo.GetCustomAttributes(true).OfType<ColumnDefinitionAttribute>().FirstOrDefault();

                    if (attr != null && attr.EnumType == null && !string.IsNullOrEmpty(attr.EntityProperty) && c.GetProperty(attr.EntityProperty) != null && (string.IsNullOrEmpty(attr.DisplayValue) || string.IsNullOrEmpty(attr.DisplayText)) && ((attr.Load && attr.Level >= level) || attr.Load || !lazyLoading))
                    {
                        try
                        {
                            var entity = c.GetProperty(attr.EntityProperty).GetValue(this, null);

                            if (entity != null && entity.GetType().GetInterfaces().Count(i => i.Name.Contains("ICollection")) > 0)
                            {
                                var parentKeyValue = new KeyValue { Key = this.Id, Value = string.Format("{0}Id", model.GetType().Name) };

                                AInBox.Astove.Core.Options.Filter filter = FilterFactory.GenerateFilter(container, null, propInfo.PropertyType.GetGenericArguments()[0], parentKeyValue);

                                var miTPList = typeof(ICollectionExtensions).GetMethod("ToPaginatedList");
                                var miToPaginatedList = miTPList.MakeGenericMethod(new[] { entity.GetType().GetGenericArguments()[0] });

                                // Setting parentId
                                var paginatedList = miToPaginatedList.Invoke(entity, new object[] { entity, container, filter, parentKeyValue, 1, DataEntityAttribute.DefaultPageSizes[0] });

                                var mi = typeof(PaginatedList<>).MakeGenericType(entity.GetType().GetGenericArguments()[0]).GetMethod("ToPaginatedModel");
                                var miToPaginatedModel = mi.MakeGenericMethod(new[] { propInfo.PropertyType.GetGenericArguments()[0] });
                                var paginatedModel = miToPaginatedModel.Invoke(paginatedList, new object[] { attr.WhereClause, attr.OrderBy });
                                propInfo.SetValue(model, paginatedModel, null);
                            }
                            else if (entity != null)
                            {
                                entity = (BaseEntity)entity;
                                var methodInfo = c.GetMethod("ToModel");
                                var toModelMethod = methodInfo.MakeGenericMethod(new[] { propInfo.PropertyType });

                                propInfo.SetValue(model, toModelMethod.Invoke(entity, new object[] { container, true, level + 1 }), null);
                            }
                        }
                        catch
                        {
                            throw new Exception(string.Format("Falha ao obter um objeto de navegação, propriedade {0} do tipo {1} do modelo {2}", propInfo.Name, propInfo.PropertyType.Name, typeof(TModel).Name));
                        }
                    }
                    else if (attr != null && attr.EnumType == null && !string.IsNullOrEmpty(attr.EntityProperty) && (c.GetProperty(attr.EntityProperty) == null || (!string.IsNullOrEmpty(attr.DisplayValue) && !string.IsNullOrEmpty(attr.DisplayText))) && ((attr.Load && attr.Level >= level) || attr.Load || !lazyLoading))
                    {
                        try
                        {
                            if (propInfo.PropertyType == typeof(DropDownOptions) && !string.IsNullOrEmpty(attr.DisplayValue) && !string.IsNullOrEmpty(attr.DisplayText))
                            {
                                SetDropDownOptions<TModel, TInterfaceConstraint>(model, container, propInfo, this);
                            }
                            else if (propInfo.PropertyType.GetInterfaces().Count(i => i.Name.Contains("ICollection")) > 0)
                            {
                                var ts = typeof(IEntityService<>);
                                var entityType = Type.GetType(string.Format(System.Web.Configuration.WebConfigurationManager.AppSettings["DataAssemblyFormat"], attr.EntityProperty));
                                var serviceType = ts.MakeGenericType(new[] { entityType });
                                var service = container.Resolve(serviceType);

                                var entities = service.GetType().GetMethod("GetEntities", new Type[] { }).Invoke(service, null);
                                var list = Activator.CreateInstance(typeof(HashSet<>).MakeGenericType(propInfo.PropertyType.GetGenericArguments()[0]));

                                foreach (var obj in (IEnumerable)entities)
                                {
                                    var methodInfo = obj.GetType().GetMethod("ToModel");
                                    var toModelMethod = methodInfo.MakeGenericMethod(new[] { propInfo.PropertyType.GetGenericArguments()[0] });
                                    var modelObj = toModelMethod.Invoke(obj, new object[] { container, true, level + 1 });

                                    var miAdd = list.GetType().GetMethod("Add");
                                    miAdd.Invoke(list, new object[] { modelObj });
                                }

                                propInfo.SetValue(model, list, null);
                            }
                        }
                        catch
                        {
                            throw new Exception(string.Format("Falha ao obter um objeto de navegação auxiliar, propriedade {0} do tipo {1} do modelo {2}", propInfo.Name, propInfo.PropertyType.Name, typeof(TModel).Name));
                        }
                    }
                    else if (attr != null && attr.EnumType != null && !string.IsNullOrEmpty(attr.Field))
                    {
                        try
                        {
                            var enumText = AInBox.Astove.Core.Enums.Utility.GetEnumText(attr.EnumType, Convert.ToInt32(c.GetProperty(propInfo.Name).GetValue(this, null)));
                            t.GetProperty(attr.Field.ToPascalCase()).SetValue(model, enumText, null);

                            propInfo.SetValue(model, c.GetProperty(propInfo.Name).GetValue(this, null), null);
                        }
                        catch
                        {
                            throw new Exception(string.Format("Falha ao obter um enum, propriedade {0} do tipo {1} do modelo {2}", propInfo.Name, propInfo.PropertyType.Name, typeof(TModel).Name));
                        }
                    }
                    else if (c.GetProperty(propInfo.Name) != null)
                    {
                        try
                        {
                            if (!propInfo.PropertyType.BaseType.Name.Equals("Array"))
                            {
                                propInfo.SetValue(model, c.GetProperty(propInfo.Name).GetValue(this, null), null);
                            }
                        }
                        catch
                        {
                            throw new Exception(string.Format("Falha ao obter a propriedade {0} do tipo {1} do modelo {2}", propInfo.Name, propInfo.PropertyType.Name, typeof(TModel).Name));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }

        public static TEntity ToEntityUpdate<TEntity>(IBindingModel requestModel) where TEntity : IEntity, new()
        {
            TEntity entity = new TEntity();

            Type t = typeof(TEntity);
            Type c = requestModel.GetType();
            foreach (var propInfo in c.GetProperties())
            {
                var attr = propInfo.GetCustomAttributes(true).OfType<ColumnDefinitionAttribute>().FirstOrDefault();
                if (t.GetProperty(propInfo.Name) != null)
                {
                    if (!t.GetProperty(propInfo.Name).PropertyType.IsAssignableTo<IBindingModel>())
                    {
                        t.GetProperty(propInfo.Name).SetValue(entity, propInfo.GetValue(requestModel, null), null);
                    }
                }
            }

            return entity;
        }

        public static TEntity ToEntity<TEntity>(IBindingModel requestModel) where TEntity : IEntity, new()
        {
            return ToEntity<TEntity>(requestModel, null, false, 0);
        }

        public static TEntity ToEntity<TEntity>(IBindingModel requestModel, IComponentContext container, bool lazyLoading, int level) where TEntity : IEntity, new()
        {
            TEntity entity = new TEntity();

            Type t = typeof(TEntity);
            if (requestModel != null)
            {
                Type c = requestModel.GetType();
                foreach (var propInfo in c.GetProperties())
                {
                    var attr = propInfo.GetCustomAttributes(true).OfType<ColumnDefinitionAttribute>().FirstOrDefault();
                    if (t.GetProperty(propInfo.Name) != null)
                    {
                        if (!t.GetProperty(propInfo.Name).PropertyType.IsAssignableTo<IBindingModel>())
                        {
                            t.GetProperty(propInfo.Name).SetValue(entity, propInfo.GetValue(requestModel, null), null);
                        }
                    }
                    else if (t.GetProperty(propInfo.Name) == null && propInfo.PropertyType != typeof(DropDownOptions) && propInfo.PropertyType != typeof(DropDownStringOptions) && attr != null && t.GetProperty(attr.EntityProperty) != null)
                    {
                        if (propInfo.PropertyType.IsArray)
                        {
                            Type nestedEntityType = Type.GetType(string.Format(System.Web.Configuration.WebConfigurationManager.AppSettings["DataAssemblyFormat"], attr.EntityProperty));
                            var list = Activator.CreateInstance(typeof(HashSet<>).MakeGenericType(nestedEntityType));
                            var nestedEntity = Activator.CreateInstance(nestedEntityType);

                            foreach (var obj in (IEnumerable)propInfo.GetValue(requestModel, null))
                            {
                                Type[] signature = new[] { typeof(IBindingModel) };
                                var methodInfo = typeof(BaseEntity).GetMethod("ToEntity", signature);
                                var toEntityMethod = methodInfo.MakeGenericMethod(new[] { nestedEntityType });

                                var nestedEntityObj = toEntityMethod.Invoke(nestedEntity, new object[] { obj });

                                var miAdd = list.GetType().GetMethod("Add");
                                miAdd.Invoke(list, new object[] { nestedEntityObj });
                            }

                            t.GetProperty(attr.EntityProperty).SetValue(entity, list, null);
                        }
                        else //!propInfo.PropertyType.IsArray
                        {
                            Type[] signature = new[] { typeof(IBindingModel) };
                            var methodInfo = typeof(BaseEntity).GetMethod("ToEntity", signature);
                            var toEntityMethod = methodInfo.MakeGenericMethod(new[] { Type.GetType(string.Format(System.Web.Configuration.WebConfigurationManager.AppSettings["DataAssemblyFormat"], attr.EntityProperty)) });

                            t.GetProperty(attr.EntityProperty).SetValue(entity, toEntityMethod.Invoke(entity, new object[] { propInfo.GetValue(requestModel, null) }), null);
                        }
                    }
                }

                return entity;
            }

            return default(TEntity);            
        }

        public static void SetDropDownOptions<TModel, TInterfaceConstraint>(TModel model, IComponentContext container)
            where TModel : TInterfaceConstraint, new()
        {
            foreach (var propInfo in model.GetType().GetProperties().Where(p => p.PropertyType == typeof(DropDownOptions)).ToArray())
            {
                var attr = propInfo.GetCustomAttributes(true).OfType<ColumnDefinitionAttribute>().FirstOrDefault();
                if (propInfo.PropertyType == typeof(DropDownOptions) && attr != null && !string.IsNullOrEmpty(attr.DisplayValue) && !string.IsNullOrEmpty(attr.DisplayText))
                {
                    SetDropDownOptions<TModel, TInterfaceConstraint>(model, container, propInfo, null);
                }
            }
        }

        private static void SetDropDownOptions<TModel, TInterfaceConstraint>(TModel model, IComponentContext container, PropertyInfo propInfo, IEntity entity) 
            where TModel : TInterfaceConstraint, new()
        {
            var attr = propInfo.GetCustomAttributes(true).OfType<ColumnDefinitionAttribute>().FirstOrDefault();
            if (propInfo.PropertyType == typeof(DropDownOptions) && !string.IsNullOrEmpty(attr.DisplayValue) && !string.IsNullOrEmpty(attr.DisplayText))
            {
                Type[] types = new Type[] { typeof(string), typeof(object[]), typeof(string), typeof(object[]), typeof(string[]), typeof(string[]) };
                var ts = typeof(IEntityService<>);
                var serviceType = ts.MakeGenericType(new[] { Type.GetType(string.Format(System.Web.Configuration.WebConfigurationManager.AppSettings["DataAssemblyFormat"], attr.EntityName)) });

                string whereClause = string.Empty;
                List<object> parameters = null;
                if (!string.IsNullOrEmpty(attr.WhereClause))
                {
                    whereClause = attr.WhereClause;
                    
                    // verify if exists dynamic parameters on where clause
                    // ex.: CustomerId=={CustomerId}
                    if (entity != null)
                    {
                        parameters = new List<object>();
                        int i = 0;

                        var match = Regex.Match(attr.WhereClause, @"\{(\w+)\}");
                        while (match.Success)
                        {
                            var key = match.Groups[0].Value;
                            var propertyName = match.Groups[1].Value;
                            var value = entity.GetType().GetProperty(propertyName).GetValue(entity, null);

                            whereClause = whereClause.Replace(key, string.Format("@{0}", i));
                            parameters.Add(value);

                            match = match.NextMatch();
                            i++;
                        }
                    }
                }

                List<string> orderBy = null;
                if (!string.IsNullOrEmpty(attr.OrderBy))
                {
                    orderBy = new List<string>();
                    orderBy.AddRange(attr.OrderBy.Split(new[] { "," }, StringSplitOptions.None));
                    orderBy.ForEach(o => o = o.Trim());
                }

                var service = container.Resolve(serviceType);
                var parametersArray = (parameters!=null) ? parameters.ToArray() : null;
                var orderByArray = (orderBy!=null) ? orderBy.ToArray() : null;

                var keyValues = ((List<KeyValue>)service.GetType().GetMethod("GetEntities", types).Invoke(service, new object[] { "new ( Id as Key, " + attr.DisplayText + " as Value )", null, whereClause, parametersArray, orderByArray, null })).ToArray();
                var options = new DropDownOptions { DisplayValue = attr.DisplayValue, DisplayText = attr.DisplayText, Items = keyValues };

                propInfo.SetValue(model, options, null);
            }
        }
    }
}
