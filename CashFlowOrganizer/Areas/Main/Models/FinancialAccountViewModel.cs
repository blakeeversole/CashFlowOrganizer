using CashFlowOrganizer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowOrganizer.Areas.Main.Models
{
    public class FinancialAccountViewModel
    {
        public List<Account> AccountList { get; set; }
        public Dictionary<string, string> AccountTypeDDL
        {
            get
            {
                DropDownModels ddm = new DropDownModels();
                return ddm.AccountTypeDDL;
            }
        }
    }

    public class Account
    {
        public int SubID { get; set; }
        public int ProfileAccountID { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Descr { get; set; }
        [Required(ErrorMessage = "Account Type is required")]
        public string Type { get; set; }
        [RequireAmountGreaterThanZeroBasedOnOtherFieldValue("Type","NEG")]
        public decimal MaximumBalance { get; set; }
        [Required(ErrorMessage = "Balance is required")]
        public decimal Balance { get; set; }
        [Required]
        public bool IncludeInCashFlow { get; set; }
    }
}