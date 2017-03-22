using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using AInBox.Astove.Core.Attributes;
using AInBox.Astove.Core.Options;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Data;
using Autofac;
using System.Text;
using System.Web.Security;

namespace AInBox.Astove.Core.Filter
{
    public static class FilterFactory
    {
        public static FilterOptions GenerateFilterOptions(IComponentContext container, Type modelType, KeyValue parentId, IRequestFilter cmd)
        {
            var options = new FilterOptions { Filters = new List<PropertyValue>() };

            List<IFilter> filters = GenerateFilters(modelType);
            if (filters == null || filters.Count == 0)
                return null;

            foreach (IFilter f in filters)
            {
                if ((parentId != null && !string.IsNullOrEmpty(parentId.Value) && parentId.Value.Equals(f.Property)))
                    continue;

                options.Filters.Add(f.GetFilterOptions(container, cmd));
            }

            return options;
        }

        public static List<IFilter> GenerateFilters(Type modelType)
        {
            var filters = new List<IFilter>();
            var dict = new List<FilterDictionary>();

            var user = System.Web.HttpContext.Current.User.Identity.Name;

            if (user == null)
                return null;

            // Order the columns of this table .
            List<PropertyInfo> filterList = modelType.GetProperties().Where(p => p.GetCustomAttributes(true).OfType<FilterWebapiAttribute>().DefaultIfEmpty().FirstOrDefault() != null).OrderBy(o => o.GetCustomAttributes(true).OfType<FilterWebapiAttribute>().First().GroupOrder).ToList();

            if (filterList.Count == 0)
                return null;

            foreach (PropertyInfo prop in filterList)
            {
                foreach (Attribute att in prop.GetCustomAttributes(true).OfType<FilterWebapiAttribute>().OrderBy(f => f.Order))
                {
                    FilterWebapiAttribute filterAtt = att as FilterWebapiAttribute;
                    if (filterAtt == null)
                        continue;

                    if (!string.IsNullOrEmpty(filterAtt.Permission))
                    {
                        string[] rolesRequired = filterAtt.Permission.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        RolePrincipal principal = (RolePrincipal)System.Web.HttpContext.Current.User;
                        if (rolesRequired.Length > 0 && rolesRequired.Where(r => principal.IsInRole(r)).Count() == 0)
                            continue;
                    }

                    IFilter f = Activator.CreateInstance(filterAtt.FilterType) as IFilter;
                    f.Property = prop.Name;
                    if (string.IsNullOrEmpty(filterAtt.DisplayName))
                        f.DisplayName = f.Property;

                    var attLength = prop.GetCustomAttributes(true).OfType<StringLengthAttribute>().FirstOrDefault();
                    if (attLength != null)
                        f.Length = attLength.MaximumLength;
                    f.CopyPropertiesValue(filterAtt);

                    if (f.Internal || (dict.Count(d => d.Property == f.Property && d.Type == f.GetType().Name && d.DefaultOperator==f.DefaultOperator && d.DefaultValue.Equals(f.DefaultValue)) == 0))
                    {
                        filters.Add(f);
                        dict.Add(new FilterDictionary { Property = f.Property, Type = f.GetType().Name, DefaultOperator = f.DefaultOperator, DefaultValue = f.DefaultValue });
                    }
                }
            }

            return filters;
        }

        public static string GetWhereClause(Type modelType, FilterCondition[] filters)
        {
            if (filters == null)
                return null;

            return JoinWhereClause(modelType, filters);
        }

        private static string JoinWhereClause(Type modelType, FilterCondition[] conditions)
        {
            var attr = modelType.GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault();
            string whereOperator = (attr != null && !string.IsNullOrEmpty(attr.WhereOperator)) ? attr.WhereOperator : " && ";

            StringBuilder whereClause = new StringBuilder();
            int parameterIndex = 0;
            for (int i = 0; i < conditions.Length; i++ )
            {
                if (!conditions[i].Internal)
                {
                    IFilter filter = GetFilter(conditions[i].FilterType);
                    filter.CopyPropertiesValue(conditions[i]);
                    whereClause.AppendFormat("{0}{1}", filter.GetWhereClause(conditions[i], parameterIndex), whereOperator);
                }
                else
                {
                    IFilter filter = GetFilter(conditions[i].FilterType);
                    whereClause.AppendFormat("{0} ", filter.GetWhereClauseInternal(conditions[i], parameterIndex));
                }

                if (GetParameter(modelType, conditions[i]) != null)
                    parameterIndex++;
            }

            string where = whereClause.ToString();
            if (where.EndsWith(whereOperator))
                where = where.Substring(0, where.Length-whereOperator.Length);

            return where;
        }

        public static object[] GetParameters(Type modelType, FilterCondition[] filters)
        {
            if (filters == null)
                return null;

            List<object> parameters = new List<object>();
            for (int i = 0; i < filters.Length; i++)
            {
                IFilter filter = GetFilter(filters[i].FilterType);
                if (!filters[i].Internal)
                    filter.CopyPropertiesValue(filters[i]);

                object parameter = GetParameter(modelType, filters[i]);
                if (parameter != null)
                    parameters.Add(parameter);
            }

            return parameters.ToArray();
        }

        private static object GetParameter(Type modelType, FilterCondition condition)
        {
            IFilter filter = GetFilter(condition.FilterType);

            return filter.GetParameter(modelType, condition);
        }

        private static IFilter GetFilter(string type)
        {
            string filterType = string.Format("{0}.{1}, {2}", typeof(IFilter).Namespace, type, typeof(IFilter).Assembly.FullName);
            IFilter filter = Activator.CreateInstance(Type.GetType(filterType)) as IFilter;

            return filter;
        }

        private static FilterCondition GetFilterCondition(IRequestFilter request, int index)
        {
            return new FilterCondition
                        {
                            FilterType = request.FiltersType[index],
                            Property = request.Fields[index],
                            DefaultOperator = int.Parse(request.Operators[index]),
                            DefaultValue = request.Values[index],
                            FilterExistsColumn = (request.ExistsColumns != null) ? request.ExistsColumns[index] : null
                        };
        }

        public static AInBox.Astove.Core.Options.Filter GenerateFilter(IComponentContext container, IRequestFilter cmd, Type modelType, KeyValue parentId)
        {
            FilterOptions options = FilterFactory.GenerateFilterOptions(container, modelType, parentId, cmd);
            FilterCondition[] conditions = null;
            if (options != null)
                conditions = options.GetFilterConditions(cmd);

            return new AInBox.Astove.Core.Options.Filter { Options = options, Conditions = conditions };
        }

        private class FilterDictionary
        {
            public string Property { get; set; }
            public string Type { get; set; }
            public int DefaultOperator { get; set; }
            public object DefaultValue { get; set; }
        }
    }
}
