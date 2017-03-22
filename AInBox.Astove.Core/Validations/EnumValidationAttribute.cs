using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AInBox.Astove.Core.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class EnumValidationAttribute : ValidationAttribute
    {
        private readonly Type _enumType;

        public EnumValidationAttribute(Type enumType)
            : base(errorMessage: "O campo {0} tem que ter um valor definido em {1}.")
        {
            _enumType = enumType;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, base.ErrorMessageString, name, _enumType.Name);
        }

        public override bool IsValid(object value)
        {            
            foreach (KeyValuePair<int, string> item in AInBox.Astove.Core.Enums.Utility.GetEnumTexts(_enumType))
            {
                if (value.GetType().Equals(typeof(string)))
                {
                    string innerValue = Convert.ToString(value);
                    if (item.Value.Equals(innerValue, StringComparison.CurrentCultureIgnoreCase))
                        return true;
                }
                else if (value.GetType().Equals(typeof(int)))
                {
                    int innerValue = Convert.ToInt32(value);
                    if (item.Key == innerValue)
                        return true;
                }
            }

            return false;
        }
    }
}
