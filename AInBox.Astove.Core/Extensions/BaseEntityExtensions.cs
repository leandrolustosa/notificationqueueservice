using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Model;
using System.Reflection;
using Autofac;

namespace AInBox.Astove.Core.Extensions
{
    public static class BaseEntityExtensions
    {
        public static void CopyProperties(this IEntity entity, IBindingModel requestModel)
        {
            Type t = entity.GetType();
            Type c = requestModel.GetType();
            foreach (var propInfo in t.GetProperties())
                if (c.GetProperty(propInfo.Name) != null && !propInfo.Name.Equals("Id") && !propInfo.PropertyType.IsAssignableTo<IBindingModel>())
                    propInfo.SetValue(entity, c.GetProperty(propInfo.Name).GetValue(requestModel, null), null);
        }

        public static T CreateInstanceOf<T>(this object source) where T : class, IModel, new()
        {
            Type c = source.GetType();
            Type t = typeof(T);

            T target = Activator.CreateInstance<T>();

            foreach (var propInfo in t.GetProperties())
            {
                if (c.GetProperty(propInfo.Name) != null)
                {
                    propInfo.SetValue(target, c.GetProperty(propInfo.Name).GetValue(source, null), null);
                }
            }

            return target;
        }


    }
}
