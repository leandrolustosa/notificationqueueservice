using AInBox.Astove.Core.Validations;
using System;
using System.ComponentModel.DataAnnotations;
using WebApiDoodle.Net.Http.Client.Model;

namespace AInBox.Astove.Core.Model
{
    public class BaseBindingModel : IBindingModel, IModel
    {
        [Required]
        [Minimum(1)]
        public virtual int Id { get; set; }
        public object AngularConditions { get; set; }
    }
}
