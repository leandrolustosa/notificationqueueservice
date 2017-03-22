using System;
using AInBox.Astove.Core.Extensions;
using AInBox.Astove.Core.Options.EnumDomainValues;
using AInBox.Astove.Core.Options;

namespace AInBox.Astove.Core.Filter
{
    public class BooleanFilter : FilterBase
    {
        public BooleanFilter() : base()
        {
            this.OperatorType = typeof(Options.BooleanOperator);
            this.DefaultOperator = (int)Options.BooleanOperator.Igual;
            this.DefaultValue = string.Empty;
            this.DomainValues = AInBox.Astove.Core.Enums.Utility.GetEnumTexts(typeof(BooleanEnum)).GetKeyValues();
        }

        public KeyValue[] DomainValues { get; set; }
    }
}
