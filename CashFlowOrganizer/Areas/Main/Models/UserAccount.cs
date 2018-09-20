using CashFlowOrganizer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowOrganizer.Areas.Main.Models
{
    public class UserAccount : NewUserViewModel
    {
        [Required]
        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        public string SubscriptionStatus { get; set; }
    }
}