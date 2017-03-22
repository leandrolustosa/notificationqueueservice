using System;
using System.Linq;
using AInBox.Astove.Core.Options.EnumDomainValues;

namespace AInBox.Astove.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MaskAttribute : Attribute
    {
        public string Value { get; set; }
        
        public MaskAttribute() { }
        public MaskAttribute(string value) { this.Value = value; }        
    }
}