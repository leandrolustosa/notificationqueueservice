using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AInBox.Astove.Core.Model
{
    public class HttpResponseResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
        public int Id { get; set; }
    }
}
