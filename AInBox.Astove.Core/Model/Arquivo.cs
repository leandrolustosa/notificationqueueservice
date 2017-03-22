using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AInBox.Astove.Core.Model;

namespace AInBox.Astove.Core.Model
{
    public class Arquivo : BaseJson
    {
        public string Url { get; set; }
        public string FileName { get; set; }
    }
}