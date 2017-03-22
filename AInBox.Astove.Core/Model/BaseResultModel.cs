using System;
using WebApiDoodle.Net.Http.Client.Model;

namespace AInBox.Astove.Core.Model
{
    public class BaseResultModel : IResultModel
    {
        public bool IsValid { get; set; }

        public string Message { get; set; }

        public int StatusCode { get; set; }
    }
}
