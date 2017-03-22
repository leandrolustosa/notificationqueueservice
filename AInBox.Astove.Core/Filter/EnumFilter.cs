using System;
using System.Collections.Generic;
using AInBox.Astove.Core.Extensions;
using AInBox.Astove.Core.Options;

namespace AInBox.Astove.Core.Filter
{
    public class EnumFilter : FilterBase
    {
        public EnumFilter() : base()
        {
            this.OperatorType = typeof(Options.BooleanOperator);
            this.DefaultOperator = (int)Options.BooleanOperator.Igual;
            this.DefaultValue = string.Empty;
        }

        private Type enumType;
        public Type EnumType 
        {
            get 
            { 
                return enumType; 
            }
            set 
            { 
                enumType = value;
                this.DomainValues = AInBox.Astove.Core.Enums.Utility.GetEnumTexts(this.enumType).GetKeyValues();
            } 
        }

        public KeyValue[] DomainValues { get; set; }
    }
}
