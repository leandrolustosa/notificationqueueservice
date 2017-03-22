using System;
using WebApiDoodle.Net.Http.Client.Model;

namespace AInBox.Astove.Core.Model
{
    public class BaseModel : BaseJson, IModel, IDto
    {
        public virtual int Id { get; set; }
        public object AngularConditions { get; set; }
    }
}
