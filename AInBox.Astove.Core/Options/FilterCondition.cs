using System;
using Newtonsoft.Json;

namespace AInBox.Astove.Core.Options
{
    public class FilterCondition
    {
        public string FilterType { get; set; }
        public string Property { get; set; }
        public string DisplayName { get; set; }
        public string FilterExistsColumn { get; set; }
        public int DefaultOperator { get; set; }
        public string DisplayOperator { get; set; }
        public object DefaultValue { get; set; }
        public string DisplayValue { get; set; }

        [JsonIgnore]
        public bool Internal { get; set; }
        [JsonIgnore]
        public Type InternalType { get; set; }
        [JsonIgnore]
        public string PreCondition { get; set; }
        [JsonIgnore]
        public string PosCondition { get; set; }
    }
}
