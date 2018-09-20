using CashFlowOrganizer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowOrganizer.Areas.Main.Models
{
    public class CashFlowViewModel
    {
        public decimal TotalCashFlow { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalIncome { get; set; }
        public List<FinancialItem> ExpenseList { get; set; }
        public List<FinancialItem> IncomeList { get; set; }
        public Dictionary<string, string> TypeDDL
        {
            get
            {
                DropDownModels ddm = new DropDownModels();
                return ddm.FinanceTypeDDL;
            }
        }
        public Dictionary<string, string> DayOfMonthDDL
        {
            get
            {
                DropDownModels ddm = new DropDownModels();
                return ddm.DayOfMonthDDL;
            }
        }
        public Dictionary<string, string> DayOfWeekDDL
        {
            get
            {
                DropDownModels ddm = new DropDownModels();
                return ddm.DayOfWeekDDL;
            }
        }
        public Dictionary<string, string> DayOfTwoWeekDDL
        {
            get
            {
                DropDownModels ddm = new DropDownModels();
                return ddm.DayOfTwoWeeksDDL;
            }
        }
    }

    public class FinancialItem
    {
        public int ProfileFinanceID { get; set; }
        public int SubID { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Descr { get; set; }
        [Required(ErrorMessage = "Frequency is required")]
        public string Type { get; set; }
        [RequireStringBasedOnOtherFieldValueAttribute("Type","MON")]
        public string DayOfMonth { get; set; }
        [RequireStringBasedOnOtherFieldValueAttribute("Type", "SEM")]
        public string DayOfMonthFirst { get; set; }
        [RequireStringBasedOnOtherFieldValueAttribute("Type", "SEM")]
        public string DayOfMonthSecond { get; set; }
        [RequireStringBasedOnOtherFieldValueAttribute("Type", "WEE")]
        public string DayOfWeek { get; set; }
        [RequireStringBasedOnOtherFieldValueAttribute("Type", "BWE")]
        public string DayOfTwoWeek { get; set; }
        [RequireAmountGreaterThanZero]
        public decimal Amount { get; set; }       
    }
}