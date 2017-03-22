using System;
using System.Linq;
using AInBox.Astove.Core.Options.EnumDomainValues;

namespace AInBox.Astove.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CssClassAttribute : Attribute
    {
        public string Value { get; set; }
        
        public CssClassAttribute() { }
        public CssClassAttribute(string cssClass) { this.Value = cssClass; }        
    }
}