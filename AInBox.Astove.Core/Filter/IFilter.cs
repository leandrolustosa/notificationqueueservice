using System;
using System.Collections.Generic;

using AInBox.Astove.Core.Data;
using Autofac;

namespace AInBox.Astove.Core.Filter
{
    public interface IFilter
    {
        string DisplayName { get; set; }
        string Property { get; set; }
        Type PropertyType { get; set; }
        Type OperatorType { get; set; }
        object DefaultValue { get; set; }
        int DefaultOperator { get; set; }
        string FilterExistsColumn { get; set; }
        string CssClass { get; set; }
        string Mask { get; set; }
        int Width { get; set; }
        int Length { get; set; }
        bool PreLoaded { get; set; }
        bool Internal { get; set; }
        string Where { get; set; }

        Type InternalOperatorType { get; }
        Type InternalType { get; set; }

        void CopyPropertiesValue(object value);
        Options.PropertyValue GetFilterOptions(IComponentContext container, IRequestFilter cmd);
        string GetWhereClause(Options.FilterCondition condition, int index);
        string GetWhereClauseInternal(Options.FilterCondition condition, int index);
        object GetParameter(Type modelType, Options.FilterCondition condition);
    }
}
