using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AInBox.Astove.Core.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class MinimumAttribute : ValidationAttribute
    {
        private readonly int _minimumValue;

        public MinimumAttribute(int minimum) : base(errorMessage: "O campo {0} tem que ser no mínimo {1}.")
        {
            _minimumValue = minimum;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, base.ErrorMessageString, name, _minimumValue);
        }

        public override bool IsValid(object value)
        {
            int intValue;
            if (value != null && int.TryParse(value.ToString(), out intValue))
            {
                return (intValue >= _minimumValue);
            }
            return false;
        }
    }
}
