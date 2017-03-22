using System;

namespace AInBox.Astove.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class StringFormatMethodAttribute : Attribute
    {
        private readonly string myFormatParameterName;
        public string FormatParameterName
        {
            get
            {
                return this.myFormatParameterName;
            }
        }
        public StringFormatMethodAttribute(string formatParameterName)
        {
            this.myFormatParameterName = formatParameterName;
        }
    } 
}
