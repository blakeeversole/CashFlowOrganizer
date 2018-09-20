using CashFlowOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowOrganizer.Areas.Main.Models
{
    public class CashFlowModels
    {
        public string Name { get; set; }
        public decimal AvailableFunds { get; set; }
        public decimal TotalMoneySaved { get; set; }
        public decimal AverageMonthlyCashFlow { get; set; }

        public List<Day> DayList { get; set; }
        public List<Week> WeekList { get; set; }
        public List<Month> MonthList { get; set; }
        public List<Account> AccountList { get; set; }

        public List<IncomeExpense> TodaysExpenses { get; set; }
        public List<IncomeExpense> UpcomingExpenses { get; set; }

        public int Month { get; set; }
        public Dictionary<int, string> CashFlowMonthsDDL
        {
            get
            {
                DropDownModels ddm = new DropDownModels();
                return ddm.CashFlowMonthsDDL;
            }
        }
    }

    public class Day
    {
        public DateTime Date { get; set; }
        public decimal CombinedBalance { get; set; }
        public List<IncomeExpense> ExpenseList { get; set; }
        public List<IncomeExpense> IncomeList { get; set; }
    }

    public class Week
    {
        public string DateRange { get; set; }
        public decimal CombinedBalance { get; set; }
        public List<IncomeExpense> ExpenseList { get; set; }
        public List<IncomeExpense> IncomeList { get; set; }
    }

    public class Month
    {
        public string MonthName { get; set; }
        public decimal CombinedBalance { get; set; }
        public List<IncomeExpense> ExpenseList { get; set; }
        public List<IncomeExpense> IncomeList { get; set; }
    }

    public class IncomeExpense
    {
        public string Descr { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}