using System;
using System.Collections.Generic;
using AInBox.Astove.Core.Options;

namespace AInBox.Astove.Core.Extensions
{
    public static class FiltersExtension
    {
        public static KeyValue[] GetKeyValues(this Dictionary<int, string> enums)
        {
            List<KeyValue> list = new List<KeyValue>();
            foreach (KeyValuePair<int, string> kv in enums)
                list.Add(new KeyValue { Key = kv.Key, Value = kv.Value });

            return list.ToArray();
        }
    }
}
