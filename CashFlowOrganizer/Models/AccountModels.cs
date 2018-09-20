using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowOrganizer.Models
{
    public class SignInViewModel
    {
        public int ProfileID { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Confirm Email")]
        [DataType(DataType.EmailAddress)]
        public string ConfirmEmail { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    public class NewUserViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        //public Billing Billing { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept our Terms and Conditions and our Privacy Policy in order to create an account.")]
        public bool TermsAndConditions { get; set; }
    }    

    //public class Billing
    //{
    //    [Required]
    //    [Display(Name = "Card Number")]
    //    [DataType(DataType.CreditCard)]
    //    public string CardNumber { get; set; }        
    //    [Required]
    //    [Display(Name = "Expiration Month")]
    //    public string ExpirationMonth { get; set; }
    //    [Required]
    //    [Display(Name = "Expiration Year")]
    //    public string ExpirationYear { get; set; }
    //    [Required]
    //    [Display(Name = "Security Code")]
    //    [Range(0, int.MaxValue)]
    //    public string SecurityCode { get; set; }
    //    [Required]
    //    [Display(Name = "Address")]
    //    public string Address { get; set; }
    //    [Required]
    //    [Display(Name = "City")]
    //    public string City { get; set; }
    //    [Required]
    //    [Display(Name = "State")]
    //    public string State { get; set; }
    //    [Required]
    //    [Display(Name = "Zipcode")]
    //    public string Zipcode { get; set; }

    //    public Dictionary<string, string> MonthDDL
    //    {
    //        get
    //        {
    //            DropDownModels ddm = new DropDownModels();
    //            return ddm.MonthDDL;
    //        }
    //    }

    //    public Dictionary<string, string> YearDDL
    //    {
    //        get
    //        {
    //            DropDownModels ddm = new DropDownModels();
    //            return ddm.YearDDL;
    //        }
    //    }

    //    public Dictionary<string, string> StateDDL
    //    {
    //        get
    //        {
    //            DropDownModels ddm = new DropDownModels();
    //            return ddm.StateDDL;
    //        }
    //    }
    //}
}