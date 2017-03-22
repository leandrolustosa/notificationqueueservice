using System;
using System.Collections.Generic;
using System.Linq;
using AInBox.Astove.Core.Filter;

namespace AInBox.Astove.Core.Options
{
    public class DropDownOptions
    {
        public string DisplayValue { get; set; }
        public string DisplayText { get; set; }

        public KeyValue[] Items { get; set; }
    }
}
