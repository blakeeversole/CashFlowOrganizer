using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowOrganizer.Models
{
    public class RequireStringBasedOnOtherFieldValueAttribute : ValidationAttribute
    {
        private readonly string _otherProperty;
        private readonly string _otherValue;
        public RequireStringBasedOnOtherFieldValueAttribute(string otherProperty, string otherValue)
        {
            _otherProperty = otherProperty;
            _otherValue = otherValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = ValidationResult.Success;

            var property = validationContext.ObjectType.GetProperty(_otherProperty);
            var otherPropertyValue = property.GetValue(validationContext.ObjectInstance, null);

            if ((otherPropertyValue?.ToString() == _otherValue && value != null) || otherPropertyValue?.ToString() != _otherValue)
            {
                return validationResult;
            }
            else
            {
                return validationResult = new ValidationResult("This field is required");
            }

        }

    }
}