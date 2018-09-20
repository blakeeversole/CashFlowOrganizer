using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowOrganizer.Models
{
    public class RequireAmountGreaterThanZeroAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = ValidationResult.Success;

            if (Convert.ToDecimal(value) > 0)
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