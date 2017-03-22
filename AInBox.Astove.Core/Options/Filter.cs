using System;

namespace AInBox.Astove.Core.Options
{
    public class Filter
    {
        public FilterOptions Options { get; set; }
        public FilterCondition[] Conditions { get; set; }
    }
}
