using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AInBox.Astove.Core.Options
{
    public class SortOptions
    {
        public string[] Fields { get { return new[] { "Id" }; } }
        public string[] Directions { get { return new[] { "asc" }; } }
    }
}
