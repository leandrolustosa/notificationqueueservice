using System;
using AInBox.Astove.Core.Options;

namespace AInBox.Astove.Core.Filter
{
    public class LikeFilter : FilterBase
    {
        public LikeFilter() : base()
        {
            this.OperatorType = typeof(Options.StringOperator);
            this.DefaultOperator = (int)Options.StringOperator.Contem;
            this.DefaultValue = string.Empty;
            this.Width = 100;
        }
    }
}
