using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashFlowOrganizer.Areas.Main.Models
{
    public class RecurringModel
    {
        public string IncomeOrExpense { get; set; }
        public string Descr { get; set; }
        public string Type { get; set; }
        public DateTime NextDate { get; set; }
        public decimal Amount { get; set; }
    }
}