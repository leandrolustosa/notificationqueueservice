using System;

namespace AInBox.Astove.Core.Model
{
    public interface IModel
    {
        int Id { get; set; }
        object AngularConditions { get; set; }
    }
}
