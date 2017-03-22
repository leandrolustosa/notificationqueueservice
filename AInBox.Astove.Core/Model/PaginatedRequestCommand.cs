using System;
using System.ComponentModel.DataAnnotations;
using AInBox.Astove.Core.Validations;
using AInBox.Astove.Core.Filter;
using AInBox.Astove.Core.Options;
using AInBox.Astove.Core.Enums;
using System.Collections.Generic;

namespace AInBox.Astove.Core.Model
{
    public class PaginatedRequestCommand : IRequestCommand, IRequestFilter
    {
        public int EntityId { get; set; }

        [Minimum(1)]
        public int Page { get; set; }

        [Minimum(1)]
        [Maximum(1000)]
        public int Take { get; set; }

        public string[] OrdersBy { get; set; }
        public string[] Directions { get; set; }

        public bool HasDefaultConditions { get; set; }
        public string[] FiltersType { get; set; }
        public string[] ExistsColumns { get; set; }
        public string[] Fields { get; set; }
        public string[] Operators { get; set; }
        public string[] Values { get; set; }

        public int ParentKey { get; set; }
        public string ParentValue { get; set; }

        public Dictionary<string, object> FilterInternalDictionary { get; set; }

        [EnumValidation(typeof(GetTypes))]
        public string Type { get; set; }
    }
}
