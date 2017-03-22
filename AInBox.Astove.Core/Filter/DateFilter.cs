using System;

namespace AInBox.Astove.Core.Filter
{
    public class DateFilter : FilterBase
    {
        public DateFilter()
            : base()
        {
            this.OperatorType = typeof(Options.DateOperator);
            this.DefaultOperator = (int)Options.DateOperator.Igual;
            this.DefaultValue = string.Empty;
            this.Width = 100;
        }
    }
}
