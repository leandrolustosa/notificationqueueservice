using System;
using System.Collections.Generic;
using System.Linq;
using AInBox.Astove.Core.Filter;

namespace AInBox.Astove.Core.Options
{
    public class DropDownStringOptions
    {
        public string DisplayValue { get; set; }
        public string DisplayText { get; set; }

        public KeyValueString[] Items { get; set; }
    }
}
