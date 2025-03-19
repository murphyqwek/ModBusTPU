using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModBusTPU.ViewModels.Field
{
    public class IntValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string input && int.TryParse(input, out _))
            {
                return ValidationResult.ValidResult;
            }
            return new ValidationResult(false, "Invalid integer value.");
        }
    }

}
