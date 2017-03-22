using System;

namespace AInBox.Astove.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ValidationModelAttribute : Attribute
    {
        public string[] IgnoreProperties { get; set; }

        public ValidationModelAttribute() { }
    }
}