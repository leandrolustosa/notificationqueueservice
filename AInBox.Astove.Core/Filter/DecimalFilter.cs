using System;

namespace AInBox.Astove.Core.Filter
{
    public class DecimalFilter : FilterBase
    {
        public DecimalFilter() : base()
        {
            this.OperatorType = typeof(Options.ValueOperator);
            this.DefaultOperator = (int)Options.ValueOperator.Igual;
            this.DefaultValue = string.Empty;
            this.Width = 100;
        }
    }
}
