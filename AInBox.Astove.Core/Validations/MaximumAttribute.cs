using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AInBox.Astove.Core.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MaximumAttribute : ValidationAttribute
    {
        private readonly int _maximumValue;

        public MaximumAttribute(int maximum)
            : base(errorMessage: "O campo {0} tem que ser no máximo {1}.")
        {
            _maximumValue = maximum;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, base.ErrorMessageString, name, _maximumValue);
        }

        public override bool IsValid(object value)
        {
            int intValue;
            if (value != null && int.TryParse(value.ToString(), out intValue))
            {
                return (intValue <= _maximumValue);
            }
            return false;
        }
    }
}
