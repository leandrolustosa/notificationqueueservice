using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

namespace AInBox.Astove.Core.Model
{
    public class BaseJson
    {
        public static TModel ParseJsonFromUrl<TModel>(string url) where TModel : IModel, new()
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(url);
                }
                catch (Exception) { }
                // if string with JSON data is not empty, deserialize it to class and return its instance 
                return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<TModel>(json_data) : new TModel();
            }
        }

        public static TModel ParseJsonFromData<TModel>(string jsonData) where TModel : IModel, new()
        {
            return !string.IsNullOrEmpty(jsonData) ? JsonConvert.DeserializeObject<TModel>(jsonData) : new TModel();
        }

        public string ParseToJson()
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return JsonConvert.SerializeObject(this, settings);
        }
    }
}
