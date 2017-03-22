using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using AInBox.Astove.Core.Options;
using AInBox.Astove.Core.Extensions;
using AInBox.Astove.Core.Data;
using AInBox.Astove.Core.Attributes;
using Autofac;

namespace AInBox.Astove.Core.Filter
{
    public class FilterBase : IFilter
    {
        public string DisplayName { get; set; }
        public string Property { get; set; }
        public Type OperatorType { get; set; }
        public object DefaultValue { get; set; }
        public int DefaultOperator { get; set; }
        public string FilterExistsColumn { get; set; }
        public string CssClass { get; set; }
        public string Mask { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
        public bool PreLoaded { get; set; }
        public bool Internal { get; set; }
        public Type InternalType { get; set; }
        public Type PropertyType { get; set; }
        public string Permission { get; set; }
        public string PreCondition { get; set; }
        public string PosCondition { get; set; }
        public string Where { get; set; }
        public string OrderBy { get; set; }

        public Type InternalOperatorType
        {
            get { return Type.GetType(string.Format("{0}.Internal.{1}", this.OperatorType.Namespace, this.OperatorType.Name)); }
        }

        public Type InternalFullOperatorType
        {
            get { return Type.GetType(string.Format("{0}.Internal.Full.{1}", this.OperatorType.Namespace, this.OperatorType.Name)); }
        }

        public void CopyPropertiesValue(object value)
        {
            Type c = value.GetType();
            Type t = this.GetType();
            foreach (var propInfo in t.GetProperties())
            {
                if (c.GetProperty(propInfo.Name) != null)
                {
                    if (propInfo.Name.Equals("Property") || propInfo.Name.Equals("DisplayName") || propInfo.Name.Equals("DefaultValue"))
                    {
                        if (c.GetProperty(propInfo.Name).GetValue(value, null) != null)
                        {
                            propInfo.SetValue(this, c.GetProperty(propInfo.Name).GetValue(value, null), null);
                            if (propInfo.Name.Equals("DefaultValue") && !string.IsNullOrEmpty((string)c.GetProperty(propInfo.Name).GetValue(value, null))) 
                                this.PreLoaded = true;
                        }
                    }
                    else if ((propInfo.Name.Equals("DefaultOperator") || propInfo.Name.Equals("Width") || propInfo.Name.Equals("Length")))
                    {
                        if (Convert.ToInt32(c.GetProperty(propInfo.Name).GetValue(value, null)) != -1)
                            propInfo.SetValue(this, c.GetProperty(propInfo.Name).GetValue(value, null), null);
                    }
                    else if (propInfo.Name.Equals("CssClass"))
                    {
                        string thisCssClass = (string)t.GetProperty(propInfo.Name).GetValue(this, null);
                        string otherCssClass = (string)c.GetProperty(propInfo.Name).GetValue(value, null);
                        propInfo.SetValue(this, string.Format("{0} {1}", (!string.IsNullOrEmpty(thisCssClass)) ? thisCssClass : "", (!string.IsNullOrEmpty(otherCssClass)) ? otherCssClass : "").Trim(), null);
                    }
                    else if (propInfo.Name.Equals("InternalType"))
                    {
                        if (c.GetProperty(propInfo.Name).GetValue(value, null) != null)
                        {
                            propInfo.SetValue(this, c.GetProperty(propInfo.Name).GetValue(value, null), null);
                        }
                    }
                    else
                    {
                        propInfo.SetValue(this, c.GetProperty(propInfo.Name).GetValue(value, null), null);
                    }
                }
            }
        }

        public virtual PropertyValue GetFilterOptions(IComponentContext container, IRequestFilter cmd)
        {
            PropertyValue obj = new PropertyValue
            {
                FilterType = this.GetType().Name,
                Operators = AInBox.Astove.Core.Enums.Utility.GetEnumTexts(this.OperatorType).GetKeyValues()
            };

            Type t = obj.GetType();
            Type c = this.GetType();
            foreach (var propInfo in t.GetProperties())
                if (c.GetProperty(propInfo.Name) != null)
                    propInfo.SetValue(obj, c.GetProperty(propInfo.Name).GetValue(this, null), null);

            if (this.GetType().Equals(typeof(FKDropdownListFilter)))
            {
                var fk = this as FKDropdownListFilter;
                if (this.Internal == false && !string.IsNullOrEmpty(fk.EntityName) && fk.EntityType != null)
                {                    
                    obj.EntityName = !string.IsNullOrEmpty(fk.EntityName) ? fk.EntityName : fk.EntityType.Name.ToLower();
                    string fieldText = (this as FKDropdownListFilter).EntityType.GetCustomAttributes(true).OfType<DataEntityAttribute>().FirstOrDefault().DisplayName;
                    if (string.IsNullOrEmpty(fieldText))
                        fieldText = "Id";

                    Type[] types = new Type[] { typeof(string), typeof(object[]), typeof(string), typeof(object[]), typeof(string[]), typeof(string[]) };
                    var ts = typeof(IEntityService<>);
                    var serviceType = ts.MakeGenericType(new[] { Type.GetType(string.Format(System.Web.Configuration.WebConfigurationManager.AppSettings["DataAssemblyFormat"], obj.EntityName)) });

                    var service = container.Resolve(serviceType);
                    if (!string.IsNullOrEmpty(this.Where) && this.Where.IndexOf("@") > 0 && cmd != null && cmd.FilterInternalDictionary != null && cmd.FilterInternalDictionary.Count > 0)
                    {
                        foreach (KeyValuePair<string, object> kv in cmd.FilterInternalDictionary)
                            this.Where = this.Where.Replace(string.Concat("@", kv.Key), Convert.ToString(kv.Value));

                        if (this.Where.IndexOf("@") > 0)
                            this.Where = null;
                    }
                    else if (!string.IsNullOrEmpty(this.Where) && this.Where.IndexOf("@") > 0 && cmd == null)
                        this.Where = null;
                    else if (string.IsNullOrEmpty(this.Where))
                        this.Where = null;

                    List<string> orderBy = null;
                    if (!string.IsNullOrEmpty(this.OrderBy))
                    {
                        orderBy = new List<string>();
                        orderBy.AddRange(this.OrderBy.Split(new[] { "," }, StringSplitOptions.None));
                        orderBy.ForEach(o => o = o.Trim());
                    }

                    var orderByArray = (orderBy != null) ? orderBy.ToArray() : null;

                    obj.DomainValues = ((List<KeyValue>)service.GetType().GetMethod("GetEntities", types).Invoke(service, new object[] { "new ( Id as Key, " + fieldText + " as Value )", null, this.Where, null, orderByArray, null })).ToArray();
                }
            }

            return obj;
        }

        public virtual string GetWhereClause(Options.FilterCondition condition, int index)
        {
            string whereFormat = GetWhereClauseFormat(condition, false);
            
            string where = string.Format(whereFormat, condition.Property, index);

            return where;
        }

        public virtual string GetWhereClauseInternal(Options.FilterCondition condition, int index)
        {
            string whereFormat = GetWhereClauseFormat(condition, true);

            string preCondition = (string.IsNullOrEmpty(condition.PreCondition)) ? string.Empty : condition.PreCondition;
            string posCondition = (string.IsNullOrEmpty(condition.PosCondition)) ? string.Empty : condition.PosCondition;

            string where = string.Format(whereFormat, preCondition, condition.Property, index, posCondition);

            return where;
        }

        private string GetWhereClauseFormat(Options.FilterCondition condition, bool full)
        {
            string value = Convert.ChangeType(condition.DefaultValue, typeof(string)) as string;
            if (!(AInBox.Astove.Core.Enums.Utility.GetEnumText(this.InternalOperatorType, condition.DefaultOperator).Contains("null")) && (string.IsNullOrEmpty(value) || value.Equals("-1")))
                return string.Empty;

            string whereFormat = string.Empty;
            whereFormat = AInBox.Astove.Core.Enums.Utility.GetEnumText((!full) ? this.InternalOperatorType : this.InternalFullOperatorType, condition.DefaultOperator);

            return whereFormat;
        }

        public virtual object GetParameter(Type modelType, FilterCondition condition)
        {
            object parameter = null;

            string value = Convert.ChangeType(condition.DefaultValue, typeof(string)) as string;
            if (!(AInBox.Astove.Core.Enums.Utility.GetEnumText(this.InternalOperatorType, condition.DefaultOperator).Contains("null")) && (string.IsNullOrEmpty(value) || value.Equals("-1")))
                return null;

            string where = AInBox.Astove.Core.Enums.Utility.GetEnumText(this.InternalOperatorType, condition.DefaultOperator);
            if (where.IndexOf("@") < 0)
                return null;

            object obj = (Convert.ToString(condition.DefaultValue)).Replace("\"", "") as object;
            if (condition.FilterType.Equals("BooleanFilter"))
                obj = (value.Equals("0")) ? "false" : "true";

            try
            {
                Type type = modelType.GetProperty(condition.Property).PropertyType;
                if (type.GetGenericArguments() != null && type.GetGenericArguments().Length > 0)
                    type = type.GetGenericArguments()[0];

                parameter = Convert.ChangeType(obj, type);
            }
            catch
            {
                if (condition.InternalType!=null)
                    parameter = Convert.ChangeType(obj, condition.InternalType);
                else if (this.PropertyType!=null)
                    parameter = Convert.ChangeType(obj, this.PropertyType);
            }

            return parameter;
        }
    }
}
