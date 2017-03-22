using System;
using System.Collections.Generic;
using System.Linq;
using AInBox.Astove.Core.Filter;

namespace AInBox.Astove.Core.Options
{
    public class FilterOptions
    {
        public List<PropertyValue> Filters { get; set; }

        public FilterCondition[] GetFilterConditions(IRequestFilter cmd)
        {
            List<FilterCondition> conditions = new List<FilterCondition>();

            if (cmd != null && cmd.Fields != null)
            {
                for (int i = 0; i < cmd.Fields.Length; i++)
                    conditions.Add(
                        new FilterCondition
                        {
                            FilterType = cmd.FiltersType[i],
                            Property = cmd.Fields[i],
                            DisplayName = this.GetDisplayName(cmd.Fields[i]),
                            DefaultOperator = int.Parse(cmd.Operators[i]),
                            DisplayOperator = this.GetDisplayOperator(cmd.Fields[i], cmd.Operators[i]),
                            DefaultValue = cmd.Values[i],
                            DisplayValue = this.GetDisplayValue(cmd.FiltersType[i], cmd.Fields[i], cmd.Values[i]),                            
                            FilterExistsColumn = (cmd.ExistsColumns != null) ? cmd.ExistsColumns[i] : string.Empty
                        });

                conditions.AddRange(GetInternalFilterConditions(cmd));
            }
            else if (cmd == null || cmd.HasDefaultConditions || cmd.FilterInternalDictionary.Count > 0)
            {
                conditions = this.GetDefaultFilterConditions(cmd);
            }

            return (conditions != null) ? conditions.ToArray() : new FilterCondition[0];
        }

        private string GetDisplayName(string propertyName)
        {
            PropertyValue pv = this.Filters.FirstOrDefault(f => f.Property.Equals(propertyName));
            return pv.DisplayName;
        }

        private string GetDisplayOperator(string propertyName, string operatorValue)
        {
            PropertyValue pv = this.Filters.FirstOrDefault(f => f.Property.Equals(propertyName));
            return pv.Operators[int.Parse(operatorValue)].Value;
        }

        private string GetDisplayValue(string filterType, string propertyName, string value)
        {
            if (string.IsNullOrEmpty(value) || value.Equals("null", StringComparison.InvariantCultureIgnoreCase))
                return string.Empty;

            string displayValue = string.Empty;

            PropertyValue pv = this.Filters.FirstOrDefault(f => f.Property.Equals(propertyName));
            if (pv.DomainValues != null)
            {
                var domainValue = pv.DomainValues.FirstOrDefault(d => d.Key == int.Parse(value));
                displayValue = domainValue.Value;
            }
            else if (filterType.Equals("DateFilter", StringComparison.InvariantCultureIgnoreCase))
            {
                var date = (DateTime)Convert.ChangeType((object)value.Replace("\"", ""), typeof(DateTime));
                displayValue = date.ToString("dd/MM/yyyy");
            }
            else
            {
                displayValue = value;
            }

            return displayValue;
        }

        private List<FilterCondition> GetDefaultFilterConditions(IRequestFilter cmd)
        {
            var filters = this.Filters.Where(p => p.PreLoaded == true || p.Internal==true).ToList();
            var conditions = new List<FilterCondition>();

            if (filters.Count == 0)
                return conditions;

            foreach (PropertyValue value in filters)
                if (value.Internal == false)
                    conditions.Add(this.GetFilterCondition(value, null));

            conditions.AddRange(GetInternalFilterConditions(cmd));

            return conditions;
        }

        private List<FilterCondition> GetInternalFilterConditions(IRequestFilter cmd)
        {
            var filters = this.Filters.Where(p => p.Internal == true).ToList();
            var conditions = new List<FilterCondition>();

            if (filters.Count == 0 || cmd == null || cmd.FilterInternalDictionary.Count==0)
                return conditions;

            foreach (PropertyValue value in filters)
            {
                string key = (string)value.DefaultValue;
                conditions.Add(this.GetFilterCondition(value, cmd.FilterInternalDictionary[key]));
            }

            return conditions;
        }

        private FilterCondition GetFilterCondition(PropertyValue value, object defaultValue)
        {
            FilterCondition obj = new FilterCondition();

            Type t = obj.GetType();
            Type c = value.GetType();
            foreach (var propInfo in t.GetProperties())
                if (c.GetProperty(propInfo.Name) != null)
                    propInfo.SetValue(obj, c.GetProperty(propInfo.Name).GetValue(value, null), null);

            if (defaultValue != null)
                obj.DefaultValue = defaultValue;

            obj.DisplayOperator = this.Filters.FirstOrDefault(f => f.Property.Equals(value.Property)).Operators[obj.DefaultOperator].Value;
            
            KeyValue[] domainValues = this.Filters.FirstOrDefault(f => f.Property.Equals(value.Property)).DomainValues;
            if (domainValues != null)
            {
                if (string.IsNullOrEmpty(Convert.ToString(obj.DefaultValue)))
                    obj.DisplayValue = Convert.ToString(obj.DefaultValue);
                else
                    obj.DisplayValue = domainValues[Convert.ToInt32(obj.DefaultValue)].Value;
            }
            else
            {
                obj.DisplayValue = string.Empty;
                if (obj.DefaultValue != null)
                    obj.DisplayValue = Convert.ToString(obj.DefaultValue);
            }

            return obj;
        }
    }
}
