using CashFlowOrganizer.Areas.Main.Models;
using CashFlowOrganizer.Controllers;
using CashFlowOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace CashFlowOrganizer.Areas.Main.Controllers
{
    [Authorize(Roles = "REG")]
    public class IncomeExpenseController : BaseController
    {
        public IncomeExpenseController()
        {
            db = new CashFlowOrganizerEntities();
        }

        public ActionResult Index()
        {
            CashFlowViewModel vm = new CashFlowViewModel();
            vm.IncomeList = new List<FinancialItem>();
            vm.ExpenseList = new List<FinancialItem>();

            var profileId = Session["ProfileID"];

            if (profileId == null)
                return RedirectToAction("SignOut", "Account", new { area = "" });

            var result = db.ProfileIncomeExpenseSelect(Convert.ToInt32(profileId)).FirstOrDefault();

            //Income
            if (result.IncomeXML != null)
            {
                XmlDocument incomeXML = new XmlDocument();
                incomeXML.LoadXml(result.IncomeXML);

                XmlNodeList nodesChoice = incomeXML.SelectNodes("/IncomeXML/Income");

                for (int i = 0; i < nodesChoice.Count; i++)
                {
                    FinancialItem fi = new FinancialItem();

                    fi.ProfileFinanceID = Convert.ToInt32(nodesChoice[i].SelectSingleNode("ProfileIncomeID").InnerText);
                    fi.Descr = nodesChoice[i].SelectSingleNode("Descr").InnerText;
                    fi.Type = nodesChoice[i].SelectSingleNode("Type").InnerText;
                    fi.Amount = Convert.ToDecimal(nodesChoice[i].SelectSingleNode("Amount").InnerText);
                    fi.SubID = Convert.ToInt32(nodesChoice[i].SelectSingleNode("SortOrder").InnerText);

                    string occurance = nodesChoice[i].SelectSingleNode("Occurance").InnerText;

                    if (fi.Type == "MON")
                        fi.DayOfMonth = occurance;
                    else if (fi.Type == "SEM")
                    {
                        fi.DayOfMonthFirst = occurance.Split(',').FirstOrDefault();
                        fi.DayOfMonthSecond = occurance.Split(',').LastOrDefault();
                    }
                    else if (fi.Type == "BWE")
                        fi.DayOfTwoWeek = occurance;
                    else if (fi.Type == "WEE")
                        fi.DayOfWeek = occurance;

                    vm.IncomeList.Add(fi);
                }
            }
            else
            {
                //vm.IncomeList.Add(new FinancialItem() { SubID = 1 });
            }

            //Income
            if (result.ExpenseXML != null)
            {
                XmlDocument expenseXML = new XmlDocument();
                expenseXML.LoadXml(result.ExpenseXML);

                XmlNodeList nodesChoice = expenseXML.SelectNodes("/ExpenseXML/Expense");

                for (int i = 0; i < nodesChoice.Count; i++)
                {
                    FinancialItem fi = new FinancialItem();

                    fi.SubID = i + 1;
                    fi.ProfileFinanceID = Convert.ToInt32(nodesChoice[i].SelectSingleNode("ProfileExpenseID").InnerText);
                    fi.Descr = nodesChoice[i].SelectSingleNode("Descr").InnerText;
                    fi.Type = nodesChoice[i].SelectSingleNode("Type").InnerText;
                    fi.Amount = Convert.ToDecimal(nodesChoice[i].SelectSingleNode("Amount").InnerText);
                    fi.SubID = Convert.ToInt32(nodesChoice[i].SelectSingleNode("SortOrder").InnerText);

                    string occurance = nodesChoice[i].SelectSingleNode("Occurance").InnerText;

                    if (fi.Type == "MON")
                        fi.DayOfMonth = occurance;
                    else if (fi.Type == "SEM")
                    {
                        fi.DayOfMonthFirst = occurance.Split(',').FirstOrDefault();
                        fi.DayOfMonthSecond = occurance.Split(',').LastOrDefault();
                    }
                    else if (fi.Type == "BWE")
                        fi.DayOfTwoWeek = occurance;
                    else if (fi.Type == "WEE")
                        fi.DayOfWeek = occurance;

                    vm.ExpenseList.Add(fi);
                }
            }
            else
            {
                //vm.ExpenseList.Add(new FinancialItem() { SubID = 1 });
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult UpdateCashFlow(CashFlowViewModel vm, string submitIncome, string submitExpense, string submitRefresh, string submitCheck)
        {
            if(submitCheck == "check")
                return PartialView("_CashFlowPartial", vm);

            if (vm.IncomeList != null)
                vm.IncomeList = vm.IncomeList.OrderBy(x => x.SubID).ToList();
            else
                vm.IncomeList = new List<FinancialItem>();

            if (vm.ExpenseList != null)
                vm.ExpenseList = vm.ExpenseList.OrderBy(x => x.SubID).ToList();
            else
                vm.ExpenseList = new List<FinancialItem>();

            if (submitRefresh == "refreshDDL")
            {
                ModelState.Clear();

                return PartialView("_CashFlowPartial", vm);
            }
            else if (!string.IsNullOrEmpty(submitIncome))
            {
                submitIncome = submitIncome.Replace(" ", "");

                int length = submitIncome.Length > 6 ? 6 : submitIncome.Length;

                switch (submitIncome.Substring(0, length))
                {
                    case "Add":
                        vm.IncomeList.Add(new FinancialItem() { SubID = (vm?.IncomeList?.Count ?? 0) + 1 });

                        ModelState.Clear();

                        return PartialView("_CashFlowPartial", vm);

                    case "Remove":
                        int id = 0;
                        if (submitIncome.Length > 6)
                            id = Convert.ToInt32(submitIncome.Substring(6, (submitIncome.Length - 6)));
                        vm.IncomeList.Remove(vm.IncomeList.Where(x => x.SubID == id).FirstOrDefault());

                        for (int i = 0; i < vm.IncomeList.Count; i++)
                        {
                            vm.IncomeList[i].SubID = i + 1;
                        }

                        ModelState.Clear();

                        return PartialView("_CashFlowPartial", vm);
                }
            }
            else if (!string.IsNullOrEmpty(submitExpense))
            {
                submitExpense = submitExpense.Replace(" ", "");

                int length = submitExpense.Length > 6 ? 6 : submitExpense.Length;

                switch (submitExpense.Substring(0, length))
                {
                    case "Add":
                        vm.ExpenseList.Add(new FinancialItem() { SubID = (vm?.ExpenseList?.Count ?? 0) + 1 });

                        ModelState.Clear();

                        return PartialView("_CashFlowPartial", vm);
            
                    case "Remove":
                        int id = 0;
                        if (submitExpense.Length > 6)
                            id = Convert.ToInt32(submitExpense.Substring(6, (submitExpense.Length - 6)));
                        vm.ExpenseList.Remove(vm.ExpenseList.Where(x => x.SubID == id).FirstOrDefault());

                        for (int i = 0; i < vm.ExpenseList.Count; i++)
                        {
                            vm.ExpenseList[i].SubID = i + 1;
                        }

                        ModelState.Clear();

                        return PartialView("_CashFlowPartial", vm);
                }
            }

            if (ModelState.IsValid)
            {
                //INCOME
                XDocument incomeXML = new XDocument(new XElement("root"));
                foreach (var item in vm.IncomeList)
                {
                    string occurance = "";

                    if (item.Type == "MON")
                        occurance = item.DayOfMonth;
                    else if (item.Type == "SEM")
                        occurance = item.DayOfMonthFirst + "," + item.DayOfMonthSecond;
                    else if (item.Type == "BWE")
                        occurance = item.DayOfTwoWeek;
                    else if (item.Type == "WEE")
                        occurance = item.DayOfWeek;

                    incomeXML.Element("root").Add(
                        new XElement("row",
                            new XElement("ProfileIncomeID", item.ProfileFinanceID),
                            new XElement("Descr", item.Descr),
                            new XElement("Type", item.Type),
                            new XElement("Occurance", occurance),
                            new XElement("Amount", item.Amount),
                            new XElement("SortOrder", item.SubID)
                        )
                    );
                }

                //EXPENSES
                XDocument expenseXML = new XDocument(new XElement("root"));
                foreach (var item in vm.ExpenseList)
                {
                    string occurance = "";

                    if (item.Type == "MON")
                        occurance = item.DayOfMonth;
                    else if (item.Type == "SEM")
                        occurance = item.DayOfMonthFirst + "," + item.DayOfMonthSecond;
                    else if (item.Type == "BWE")
                        occurance = item.DayOfTwoWeek;
                    else if (item.Type == "WEE")
                        occurance = item.DayOfWeek;

                    expenseXML.Element("root").Add(
                        new XElement("row",
                            new XElement("ProfileExpenseID", item.ProfileFinanceID),
                            new XElement("Descr", item.Descr),
                            new XElement("Type", item.Type),
                            new XElement("Occurance", occurance),
                            new XElement("Amount", item.Amount),
                            new XElement("SortOrder", item.SubID)
                        )
                    );
                }

                var income = incomeXML.ToString();
                var expen = expenseXML.ToString();

                var profileId = Session["ProfileID"];

                if (profileId == null)
                    return RedirectToAction("SignOut", "Account", new { area = "" });

                var storedProcedure = db.ProfileIncomeExpenseInsertUpdate(Convert.ToInt32(profileId), income, expen).ToList();

                if (storedProcedure.FirstOrDefault().ErrorNumber.HasValue == true)
                {
                    ModelState.AddModelError("RandomError", "Error");
                    return PartialView("_CashFlowPartial", vm);
                }

                ModelState.Clear();

                TempData["msg"] = "<div class=\"col-sm-12 noPadding\"><h2 class=\"text-center greenCOLOR bold\">Successfully saved changes</h2></div>";

                return PartialView("_CashFlowPartial", vm);
            }
            else
            {
                return PartialView("_CashFlowPartial", vm);
            }


        }
    }
}