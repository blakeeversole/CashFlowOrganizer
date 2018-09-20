using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CashFlowOrganizer.Models
{
    public class IncomeExpenseViewModel
    {
        [Display(Name = "Total Expenses")]
        public decimal TotalExpenses { get; set; }
        [Display(Name = "Total Income")]
        public decimal TotalIncome { get; set; }
        [Display(Name = "Cash Flow")]
        public decimal? TotalCashFlow { get; set; }
        public List<FinancialItemTest> IncomeList { get; set; }
        public List<FinancialItemTest> ExpenseList { get; set; }

        public List<FinancialItemTest> TestIncomeList()
        {
            List<FinancialItemTest> list = new List<FinancialItemTest>();

            list.Add(new FinancialItemTest()
            {
                SubID = 1,
                Descr = "Day Job",
                Amount = Convert.ToDecimal(2000)
            });

            list.Add(new FinancialItemTest()
            {
                SubID = 2,
                Descr = "Part-Time Job",
                Amount = Convert.ToDecimal(500)
            });

            return list;
        }

        public List<FinancialItemTest> TestExpenseList()
        {
            List<FinancialItemTest> list = new List<FinancialItemTest>();

            list.Add(new FinancialItemTest()
            {
                SubID = 1,
                Descr = "Rent",
                Amount = Convert.ToDecimal(900)
            });

            list.Add(new FinancialItemTest()
            {
                SubID = 2,
                Descr = "Groceries",
                Amount = Convert.ToDecimal(500)
            });

            list.Add(new FinancialItemTest()
            {
                SubID = 2,
                Descr = "Utilities",
                Amount = Convert.ToDecimal(200)
            });

            list.Add(new FinancialItemTest()
            {
                SubID = 2,
                Descr = "Car",
                Amount = Convert.ToDecimal(215)
            });

            list.Add(new FinancialItemTest()
            {
                SubID = 2,
                Descr = "Insurance",
                Amount = Convert.ToDecimal(100)
            });

            return list;
        }
    }

    public class FinancialItemTest
    {
        [Display(Name = "Description")]
        public string Descr { get; set; }
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
        public int SubID { get; set; }
    }
}