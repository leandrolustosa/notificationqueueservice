using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AInBox.Astove.Core.Attributes;
using AInBox.Astove.Core.Options;
using AInBox.Astove.Core.Options.EnumDomainValues;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Extensions;

namespace AInBox.Astove.Core.List
{
    public class ColumnFactory
    {
        public static List<ColumnDefinition> GenerateColumnDefinitions<T>(ActionEnum action) where T : IModel
        {
            var columns = new List<ColumnDefinition>();
            
            List<PropertyInfo> columnList = typeof(T).GetProperties().Where(p => p.GetCustomAttributes(true).OfType<ColumnDefinitionAttribute>().DefaultIfEmpty().FirstOrDefault() != null).OrderBy(o => o.GetCustomAttributes(true).OfType<ColumnDefinitionAttribute>().First().Order).ToList();

            foreach (PropertyInfo prop in columnList)
            {
                foreach (ColumnDefinitionAttribute att in prop.GetCustomAttributes(true).OfType<ColumnDefinitionAttribute>())
                {
                    if (att == null)
                        continue;

                    if (att.Actions == null || !att.Actions.Contains(action))
                    {
                        ColumnDefinition column = new ColumnDefinition();
                        column.Field = prop.Name.ToCamelCase();
                        column.DisplayName = prop.Name;
                        column.CopyPropertiesValue(att);

                        columns.Add(column);
                    }
                }
            }

            return columns;
        }
    }
}
