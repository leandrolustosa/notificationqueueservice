using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AInBox.Astove.Core.Model
{
    public class FileModel : BaseJson, IModel
    {
        public string ControllerName { get; set; }
        public string PropertyName { get; set; }
        public string Url { get; set; }

        public int Id { get; set; }
        public object AngularConditions { get; set; }
    }
}
