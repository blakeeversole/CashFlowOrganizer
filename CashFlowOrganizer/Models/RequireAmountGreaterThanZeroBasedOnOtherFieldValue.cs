using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowOrganizer.Models
{
    public class RequireAmountGreaterThanZeroBasedOnOtherFieldValueAttribute : ValidationAttribute
    {
        private readonly string _otherProperty;
        private readonly string _otherValue;
        public RequireAmountGreaterThanZeroBasedOnOtherFieldValueAttribute(string otherProperty, string otherValue)
        {
            _otherProperty = otherProperty;
            _otherValue = otherValue;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = ValidationResult.Success;

            var property = validationContext.ObjectType.GetProperty(_otherProperty);
            var otherPropertyValue = property.GetValue(validationContext.ObjectInstance, null);

            if (Convert.ToDecimal(value) > 0 || (string)otherPropertyValue != _otherValue)
            {
                return validationResult;
            }
            else
            {
                return validationResult = new ValidationResult("Must be greater than zero");
            }

        }

    }
}