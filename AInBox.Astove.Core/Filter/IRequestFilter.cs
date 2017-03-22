using System;
using AInBox.Astove.Core.Options;
using System.Collections.Generic;

namespace AInBox.Astove.Core.Filter
{
    public interface IRequestFilter
    {
        int Page { get; set; }
        int Take { get; set; }
        bool HasDefaultConditions { get; set; }
        int ParentKey { get; set; }
        string ParentValue { get; set; }
        string[] FiltersType { get; set; }
        string[] ExistsColumns { get; set; }
        string[] Fields { get; set; }
        string[] Operators { get; set; }
        string[] Values { get; set; }

        Dictionary<string, object> FilterInternalDictionary { get; set; }
    }
}
