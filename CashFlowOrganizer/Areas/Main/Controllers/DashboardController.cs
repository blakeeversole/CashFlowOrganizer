using CashFlowOrganizer.Areas.Main.Models;
using CashFlowOrganizer.Controllers;
using CashFlowOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace CashFlowOrganizer.Areas.Main.Controllers
{
    [Authorize(Roles = "REG")]
    public class DashboardController : BaseController
    {
        public DashboardController()
        {
            db = new CashFlowOrganizerEntities();
        }

        public ActionResult Index()
        {
            CashFlowModels vm = new CashFlowModels();
            vm.DayList = new List<Day>();
            vm.WeekList = new List<Week>();
            vm.MonthList = new List<Month>();
            vm.AccountList = new List<Account>();
            vm.TodaysExpenses = new List<IncomeExpense>();
            vm.UpcomingExpenses = new List<IncomeExpense>();
            vm.Month = 3;

            var profileId = Session["ProfileID"];

            if (profileId == null)
                return RedirectToAction("SignOut", "Account", new { area = "" });

            var profileInfo = db.ProfileInfoSelect((int)profileId).FirstOrDefault();

            vm.Name = profileInfo.FirstName;
            vm.AvailableFunds = profileInfo.TotalAvailableFunds;
            vm.TotalMoneySaved = profileInfo.TotalMoneySaved;
            vm.AverageMonthlyCashFlow = profileInfo.AverageMonthlyCashFlow;

            /* ACCOUNTS */
            vm.AccountList = GetAccounts((int)profileId);

            /* INITIATE CASH FLOW */
            var list = GetRecurringIncomeAndExpenses((int)profileId, vm.Month);

            /* UPCOMING EXPENSES */
            vm.UpcomingExpenses = GetUpcomingExpenses(list);

            /* TODAY'S EXPENSES */
            vm.TodaysExpenses = GetTodaysExpenses(list);

            if (vm.AccountList.Where(x => x.IncludeInCashFlow).Count() > 0)
            {
                decimal startingBalance = GetStartingBalance(vm.AccountList);

                /* DAY LIST */
                vm.DayList = GetDays(list, startingBalance);

                if(vm.DayList.Count > 0)
                {
                    /* WEEK LIST */
                    vm.WeekList = GetWeeks(vm.DayList, startingBalance, vm.Month);

                    /* MONTH LIST */
                    vm.MonthList = GetMonths(vm.DayList, startingBalance, vm.Month);
                }                
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult UpdateDashboard(int NumberOfMonths)
        {
            CashFlowModels vm = new CashFlowModels();
            vm.DayList = new List<Day>();
            vm.WeekList = new List<Week>();
            vm.MonthList = new List<Month>();
            vm.AccountList = new List<Account>();

            vm.Month = NumberOfMonths;

            var profileId = Session["ProfileID"];

            if (profileId == null)
                return RedirectToAction("SignOut", "Account", new { area = "" });

            /* ACCOUNTS */
            vm.AccountList = GetAccounts((int)profileId);

            /* INITIATE CASH FLOW */
            var list = GetRecurringIncomeAndExpenses((int)profileId, vm.Month);

            decimal startingBalance = GetStartingBalance(vm.AccountList);

            /* DAY LIST */
            vm.DayList = GetDays(list, startingBalance);

            if (vm.DayList.Count > 0)
            {
                /* WEEK LIST */
                vm.WeekList = GetWeeks(vm.DayList, startingBalance, vm.Month);

                /* MONTH LIST */
                vm.MonthList = GetMonths(vm.DayList, startingBalance, vm.Month);
            }

            return PartialView("_CashFlowViews", vm);
        }


        public decimal GetStartingBalance(List<Account> AccountList)
        {
            decimal startingBalance = (AccountList?.Where(x => x.IncludeInCashFlow && x.Type == "POS")?.Select(x => x.Balance)?.Sum() ?? 0) + (AccountList?.Where(x => x.IncludeInCashFlow && x.Type == "NEG")?.Select(x => x.MaximumBalance - x.Balance)?.Sum() ?? 0);

            return startingBalance;
        }

        public List<Month> GetMonths(List<Day> DayList, decimal startingBalance, int numberOfMonths)
        {
            List<Month> MonthList = new List<Month>();

            /************* MONTH *************/
            List<KeyValuePair<int, int>> months = new List<KeyValuePair<int, int>>();

            int startMonth = DateTime.Today.Month;
            int startYear = DateTime.Today.Year;
            int endMonth = DateTime.Today.AddMonths(numberOfMonths).Month;
            int endYear = DateTime.Today.AddMonths(numberOfMonths).Year;


            int monthCount = startMonth;
            int yearCount = startYear;

            while (monthCount <= endMonth && yearCount <= endYear)
            {
                var value = new KeyValuePair<int, int>(monthCount, yearCount);
                months.Add(value);

                if (monthCount == 12)
                    yearCount += 1;

                monthCount = ((monthCount + 1) % 13) == 0 ? ((monthCount + 1) % 13) + 1 : ((monthCount + 1) % 13);
            }

            foreach (var item in months)
            {
                Month m = new Month();
                m.ExpenseList = new List<IncomeExpense>();
                m.IncomeList = new List<IncomeExpense>();

                m.MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key) + " " + item.Value;
                var days = DayList.Where(x => x.Date.Month == item.Key && x.Date.Year == item.Value);

                m.CombinedBalance = days?.LastOrDefault()?.CombinedBalance ?? startingBalance;
                startingBalance = m.CombinedBalance;

                foreach (var day in days)
                {
                    m.ExpenseList.AddRange(day.ExpenseList);
                    m.IncomeList.AddRange(day.IncomeList);
                }

                MonthList.Add(m);
            }

            return MonthList;
        }

        public List<Week> GetWeeks(List<Day> DayList, decimal startingBalance, int numberOfMonths)
        {
            List<Week> WeekList = new List<Week>();

            /************* WEEK *************/
            Dictionary<DateTime, DateTime> weeks = new Dictionary<DateTime, DateTime>();

            DateTime startWeekDay = DateTime.Today;
            DateTime endWeekDay = DateTime.Today.AddDays(6 - (int)DateTime.Today.DayOfWeek);
            weeks.Add(startWeekDay, endWeekDay);

            DateTime lastDay = DateTime.Today.AddMonths(numberOfMonths);
            DateTime countDay = endWeekDay.AddDays(1);
            while (countDay <= lastDay)
            {
                weeks.Add(countDay, countDay.AddDays(7));

                countDay = countDay.AddDays(8);
            }

            decimal latestBalance = startingBalance;
            foreach (var item in weeks)
            {
                Week w = new Week();
                w.ExpenseList = new List<IncomeExpense>();
                w.IncomeList = new List<IncomeExpense>();

                w.DateRange = item.Key.ToString("M/dd/yyyy") + " - " + item.Value.ToString("M/dd/yyy");
                var days = DayList.Where(x => x.Date >= item.Key && x.Date <= item.Value).ToList();

                w.CombinedBalance = days?.LastOrDefault()?.CombinedBalance ?? startingBalance;
                startingBalance = w.CombinedBalance;

                latestBalance = w.CombinedBalance;

                foreach (var day in days)
                {
                    w.ExpenseList.AddRange(day.ExpenseList);
                    w.IncomeList.AddRange(day.IncomeList);

                }

                WeekList.Add(w);
            }

            return WeekList;
        }

        public List<Day> GetDays(List<RecurringModel> list, decimal startingBalance)
        {
            List<Day> DayList = new List<Day>();

            List<DateTime> distinctDates = list.Select(x => x.NextDate).Distinct().ToList();

            /************* DAY *************/
            decimal dailyBalance = startingBalance;
            for (int i = 0; i < distinctDates.Count; i++)
            {
                Day d = new Day();
                d.ExpenseList = new List<IncomeExpense>();
                d.IncomeList = new List<IncomeExpense>();

                d.Date = distinctDates[i];

                var items = list.Where(x => x.NextDate == distinctDates[i]).ToList();

                for (int x = 0; x < items.Count(); x++)
                {
                    IncomeExpense ie = new IncomeExpense();

                    ie.Descr = items[x].Descr;
                    ie.Type = items[x].Type;
                    ie.Date = items[x].NextDate;
                    ie.Amount = items[x].Amount;

                    if (items[x].IncomeOrExpense == "INCOME")
                        d.IncomeList.Add(ie);
                    else
                        d.ExpenseList.Add(ie);
                }

                dailyBalance = dailyBalance + (d.IncomeList.Select(x => x.Amount).Sum()) - (d.ExpenseList.Select(x => x.Amount).Sum());

                d.CombinedBalance = dailyBalance;

                DayList.Add(d);
            }

            return DayList;
        }

        public List<IncomeExpense> GetUpcomingExpenses(List<RecurringModel> list)
        {
            List<IncomeExpense> upcomingExpenses = new List<IncomeExpense>();

            int counter = 0;
            var upcomingList = list.Where(x => x.IncomeOrExpense == "EXPENSE" && x.NextDate != DateTime.Today).ToList();
            while (counter < 3 && counter < upcomingList.Count())
            {
                IncomeExpense ie = new IncomeExpense();
                ie.Descr = upcomingList[counter].Descr;
                ie.Amount = upcomingList[counter].Amount;
                ie.Date = upcomingList[counter].NextDate;

                upcomingExpenses.Add(ie);

                counter++;
            }

            return upcomingExpenses;
        }

        public List<IncomeExpense> GetTodaysExpenses(List<RecurringModel> list)
        {
            List<IncomeExpense> todaysExpenses = new List<IncomeExpense>();

            foreach (var item in list.Where(x => x.IncomeOrExpense == "EXPENSE" && x.NextDate == DateTime.Today))
            {
                IncomeExpense ie = new IncomeExpense();
                ie.Descr = item.Descr;
                ie.Amount = item.Amount;
                ie.Date = item.NextDate;

                todaysExpenses.Add(ie);
            }

            return todaysExpenses;
        }

        public List<Account> GetAccounts(int profileId)
        {
            List<Account> list = new List<Account>();

            var result = db.ProfileAccountSelect(profileId).FirstOrDefault();

            //Accounts
            if (result != null)
            {
                XmlDocument accountXML = new XmlDocument();
                accountXML.LoadXml(result);

                XmlNodeList nodesChoice = accountXML.SelectNodes("/AccountXML/Account");

                for (int i = 0; i < nodesChoice.Count; i++)
                {
                    Account a = new Account();

                    a.Descr = nodesChoice[i].SelectSingleNode("Descr").InnerText;
                    a.Type = nodesChoice[i].SelectSingleNode("Type").InnerText;
                    a.Balance = Convert.ToDecimal(nodesChoice[i].SelectSingleNode("Balance").InnerText);
                    a.MaximumBalance = nodesChoice[i].SelectSingleNode("MaxBalance")?.InnerText == null ? 0 : Convert.ToDecimal(nodesChoice[i].SelectSingleNode("MaxBalance").InnerText);
                    a.IncludeInCashFlow = Convert.ToInt32(nodesChoice[i].SelectSingleNode("IncludeInCashFlow")?.InnerText) == 1 ? true : false;
                    a.SubID = Convert.ToInt32(nodesChoice[i].SelectSingleNode("SortOrder").InnerText);

                    list.Add(a);
                }
            }

            return list;
        }        

        public List<RecurringModel> GetRecurringIncomeAndExpenses(int ProfileID, int NumberOfMonths)
        {
            List<RecurringModel> rm = new List<RecurringModel>();

            var result = db.ProfileIncomeExpenseSelect(Convert.ToInt32(ProfileID)).FirstOrDefault();

            if (result.IncomeXML != null)
            {
                XmlDocument incomeXML = new XmlDocument();
                incomeXML.LoadXml(result.IncomeXML);

                XmlNodeList nodesChoice = incomeXML.SelectNodes("/IncomeXML/Income");

                for (int i = 0; i < nodesChoice.Count; i++)
                {
                    RecurringModel r = new RecurringModel();

                    r.IncomeOrExpense = "INCOME";
                    r.Descr = nodesChoice[i].SelectSingleNode("Descr").InnerText;
                    r.Type = nodesChoice[i].SelectSingleNode("Type").InnerText;
                    r.Amount = Convert.ToDecimal(nodesChoice[i].SelectSingleNode("Amount").InnerText);

                    string occurance = nodesChoice[i].SelectSingleNode("Occurance").InnerText;

                    if (r.Type == "MON")
                        r.NextDate = GetNextMonthlyDate(Convert.ToInt32(occurance));
                    else if (r.Type == "SEM")
                        r.NextDate = GetNextMonthlyDate(Convert.ToInt32(occurance.Split(',').FirstOrDefault()));
                    else if (r.Type == "BWE")
                        r.NextDate = GetNextBiWeeklyDate(occurance.Substring(0, 3), occurance.Substring(3, occurance.Length - 4));
                    else if (r.Type == "WEE")
                        r.NextDate = GetNextWeeklyDate(occurance);
                    else if (r.Type == "DAY")
                        r.NextDate = DateTime.Now;

                    rm.Add(r);

                    if (r.Type == "SEM")
                    {
                        RecurringModel n = new RecurringModel();

                        n.IncomeOrExpense = "INCOME";
                        n.Descr = nodesChoice[i].SelectSingleNode("Descr").InnerText;
                        n.Type = nodesChoice[i].SelectSingleNode("Type").InnerText;
                        n.Amount = Convert.ToDecimal(nodesChoice[i].SelectSingleNode("Amount").InnerText);
                        n.NextDate = GetNextMonthlyDate(Convert.ToInt32(occurance.Split(',').LastOrDefault()));

                        rm.Add(n);
                    }
                }
            }

            if (result.ExpenseXML != null)
            {
                XmlDocument expenseXML = new XmlDocument();
                expenseXML.LoadXml(result.ExpenseXML);

                XmlNodeList nodesChoice = expenseXML.SelectNodes("/ExpenseXML/Expense");

                for (int i = 0; i < nodesChoice.Count; i++)
                {
                    RecurringModel r = new RecurringModel();

                    r.IncomeOrExpense = "EXPENSE";
                    r.Descr = nodesChoice[i].SelectSingleNode("Descr").InnerText;
                    r.Type = nodesChoice[i].SelectSingleNode("Type").InnerText;
                    r.Amount = Convert.ToDecimal(nodesChoice[i].SelectSingleNode("Amount").InnerText);

                    string occurance = nodesChoice[i].SelectSingleNode("Occurance").InnerText;

                    if (r.Type == "MON")
                        r.NextDate = GetNextMonthlyDate(Convert.ToInt32(occurance));
                    else if (r.Type == "SEM")
                        r.NextDate = GetNextMonthlyDate(Convert.ToInt32(occurance.Split(',').FirstOrDefault()));
                    else if (r.Type == "BWE")
                        r.NextDate = GetNextBiWeeklyDate(occurance.Substring(0, 3), occurance.Substring(3, occurance.Length - 4));
                    else if (r.Type == "WEE")
                        r.NextDate = GetNextWeeklyDate(occurance);
                    else if (r.Type == "DAY")
                        r.NextDate = DateTime.Now;

                    rm.Add(r);

                    if (r.Type == "SEM")
                    {
                        RecurringModel n = new RecurringModel();

                        n.IncomeOrExpense = "EXPENSE";
                        n.Descr = nodesChoice[i].SelectSingleNode("Descr").InnerText;
                        n.Type = nodesChoice[i].SelectSingleNode("Type").InnerText;
                        n.Amount = Convert.ToDecimal(nodesChoice[i].SelectSingleNode("Amount").InnerText);
                        n.NextDate = GetNextMonthlyDate(Convert.ToInt32(occurance.Split(',').LastOrDefault()));

                        rm.Add(n);
                    }
                }
            }

            XDocument recurringXML = new XDocument(new XElement("root"));
            foreach (var item in rm)
            {
                recurringXML.Element("root").Add(
                    new XElement("row",
                        new XElement("IncomeOrExpense", item.IncomeOrExpense),
                        new XElement("Descr", item.Descr),
                        new XElement("Type", item.Type),
                        new XElement("NextDate", item.NextDate.Date),
                        new XElement("Amount", item.Amount)
                    )
                );
            }

            var list = db.IncomeAndExpenseDatesSelect(NumberOfMonths, recurringXML.ToString()).ToList();

            List<RecurringModel> finalRM = new List<RecurringModel>();

            foreach (var item in list)
            {
                RecurringModel r = new RecurringModel();

                r.IncomeOrExpense = item.IncomeOrExpense;
                r.Descr = item.Descr;
                r.Type = item.Type;
                r.NextDate = item.NextDate.GetValueOrDefault();
                r.Amount = item.Amount.GetValueOrDefault();

                finalRM.Add(r);
            }

            return finalRM;
        }

        public static DateTime GetNextMonthlyDate(int month)
        {
            DateTime start = DateTime.Now;
            int startDay = start.Day;
            DateTime date;

            if (month < startDay)
                date = start.AddDays(month - startDay).AddMonths(1);
            else
                date = start.AddDays(month - startDay);

            return date;
        }
        public static DateTime GetNextBiWeeklyDate(string OddOrEven, string dayOfWeek)
        {

            int day = (int)ConvertToDayOfWeek(dayOfWeek);

            DateTime date;
            DateTime start = DateTime.Today;
            int dayOfTheWeek = (int)start.DayOfWeek;

            var cal = new GregorianCalendar();
            string code;

            if (cal.GetWeekOfYear(start, CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday) % 2 == 0)
                code = "EVE";
            else
                code = "ODD";

            if (OddOrEven == code)
            {
                if (day < dayOfTheWeek)
                    date = start.AddDays(day - dayOfTheWeek + 14);
                else
                    date = start.AddDays(day - dayOfTheWeek);
            }
            else
            {
                date = start.AddDays(day - dayOfTheWeek + 7);
            }

            return date;
        }
        public static DateTime GetNextWeeklyDate(string dayOfWeek)
        {
            DayOfWeek day = ConvertToDayOfWeek(dayOfWeek);

            var start = DateTime.Today;
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }
        public static DayOfWeek ConvertToDayOfWeek(string dayOfWeek)
        {
            DayOfWeek dow;

            if (dayOfWeek == "1")
                dow = DayOfWeek.Sunday;
            else if (dayOfWeek == "2")
                dow = DayOfWeek.Monday;
            else if (dayOfWeek == "3")
                dow = DayOfWeek.Tuesday;
            else if (dayOfWeek == "4")
                dow = DayOfWeek.Wednesday;
            else if (dayOfWeek == "5")
                dow = DayOfWeek.Thursday;
            else if (dayOfWeek == "6")
                dow = DayOfWeek.Friday;
            else
                dow = DayOfWeek.Saturday;

            return dow;
        }
    }
}