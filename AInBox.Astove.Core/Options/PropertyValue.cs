using System;
using Newtonsoft.Json;

namespace AInBox.Astove.Core.Options
{
    public class PropertyValue
    {
        public string EntityName { get; set; }
        public string Property { get; set; }
        public string DisplayName { get; set; }
        public KeyValue[] Operators { get; set; }
        public int DefaultOperator { get; set; }
        public object DefaultValue { get; set; }
        public KeyValue[] DomainValues { get; set; }
        public bool PreLoaded { get; set; }
        public string FilterType { get; set; }
        public string CssClass { get; set; }
        public string Mask { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
        public string FilterExistsColumn { get; set; }
        
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
