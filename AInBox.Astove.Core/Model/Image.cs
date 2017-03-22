using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AInBox.Astove.Core.Model;

namespace AInBox.Astove.Core.Model
{
    public class Image : BaseJson
    {
        public string Path { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Dpi { get; set; }
    }
}