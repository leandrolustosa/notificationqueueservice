using System;
using AInBox.Astove.Core.Options;

namespace AInBox.Astove.Core.Filter
{
    public class FKDropdownListFilter : FilterBase
    {
        public Type EntityType { get; set; }
        public string EntityName { get; set; }
        public KeyValue[] DomainValues { get; set; }

        public FKDropdownListFilter()
            : base()
        {
            this.PropertyType = typeof(Int32);
            this.OperatorType = typeof(Options.BooleanOperator);
            this.DefaultOperator = (int)Options.BooleanOperator.Igual;
            this.DefaultValue = string.Empty;
        }
    }
}
