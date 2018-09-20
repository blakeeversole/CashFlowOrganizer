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
    public class FinancialAccountsController : BaseController
    {
        public FinancialAccountsController()
        {
            db = new CashFlowOrganizerEntities();
        }

        [HttpGet]
        public ActionResult Index()
        {
            FinancialAccountViewModel vm = new FinancialAccountViewModel();
            vm.AccountList = new List<Account>();            

            var profileId = Session["ProfileID"];

            if (profileId == null)
                return RedirectToAction("SignOut", "Account", new { area = "" });

            var result = db.ProfileAccountSelect(Convert.ToInt32(profileId)).FirstOrDefault();

            //Accounts
            if (result != null)
            {
                XmlDocument accountXML = new XmlDocument();
                accountXML.LoadXml(result);

                XmlNodeList nodesChoice = accountXML.SelectNodes("/AccountXML/Account");

                for (int i = 0; i < nodesChoice.Count; i++)
                {
                    Account a = new Account();

                    a.ProfileAccountID = Convert.ToInt32(nodesChoice[i].SelectSingleNode("ProfileAccountID").InnerText);
                    a.Descr = nodesChoice[i].SelectSingleNode("Descr").InnerText;
                    a.Type = nodesChoice[i].SelectSingleNode("Type").InnerText;
                    a.Balance = Convert.ToDecimal(nodesChoice[i].SelectSingleNode("Balance").InnerText);
                    a.MaximumBalance = nodesChoice[i].SelectSingleNode("MaxBalance")?.InnerText == null ? 0 : Convert.ToDecimal(nodesChoice[i].SelectSingleNode("MaxBalance").InnerText);
                    a.IncludeInCashFlow = Convert.ToInt32(nodesChoice[i].SelectSingleNode("IncludeInCashFlow")?.InnerText) == 1 ? true : false;
                    a.SubID = Convert.ToInt32(nodesChoice[i].SelectSingleNode("SortOrder").InnerText);

                    vm.AccountList.Add(a);
                }
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult UpdateAccounts(FinancialAccountViewModel vm, string submitAccount, string submitRefresh)
        {
            if (vm.AccountList != null)
                vm.AccountList = vm.AccountList.OrderBy(x => x.SubID).ToList();
            else
                vm.AccountList = new List<Account>();

            if (submitRefresh == "refreshDDL")
            {
                ModelState.Clear();

                return PartialView("_AccountPartial", vm);
            }
            else if (!string.IsNullOrEmpty(submitAccount))
            {
                submitAccount = submitAccount.Replace(" ", "");

                int length = submitAccount.Length > 6 ? 6 : submitAccount.Length;

                switch (submitAccount.Substring(0, length))
                {
                    case "Add":
                        vm.AccountList.Add(new Account() { SubID = (vm?.AccountList?.Count ?? 0) + 1 });

                        ModelState.Clear();

                        return PartialView("_AccountPartial", vm);

                    case "Remove":
                        int id = 0;
                        if (submitAccount.Length > 6)
                            id = Convert.ToInt32(submitAccount.Substring(6, (submitAccount.Length - 6)));
                        vm.AccountList.Remove(vm.AccountList.Where(x => x.SubID == id).FirstOrDefault());

                        for (int i = 0; i < vm.AccountList.Count; i++)
                        {
                            vm.AccountList[i].SubID = i + 1;
                        }

                        ModelState.Clear();

                        return PartialView("_AccountPartial", vm);
                }
            }
            

            if (ModelState.IsValid)
            {
                //INCOME
                XDocument accountXML = new XDocument(new XElement("root"));
                foreach (var item in vm.AccountList)
                {
                    accountXML.Element("root").Add(
                        new XElement("row",
                            new XElement("ProfileAccountID", item.ProfileAccountID),
                            new XElement("Descr", item.Descr),
                            new XElement("Type", item.Type),
                            new XElement("MaxBalance", item.MaximumBalance),
                            new XElement("Balance", item.Balance),
                            new XElement("IncludeInCashFlow", item.IncludeInCashFlow),
                            new XElement("SortOrder", item.SubID)
                        )
                    );
                }                

                var profileId = Session["ProfileID"];

                if (profileId == null)
                    return RedirectToAction("SignOut", "Account", new { area = "" });

                var storedProcedure = db.ProfileAccountInsertUpdate(Convert.ToInt32(profileId), accountXML.ToString()).ToList();

                if (storedProcedure.FirstOrDefault().ErrorNumber.HasValue == true)
                {
                    ModelState.AddModelError("RandomError", "Cannot save changes. Please contact support.");
                    return PartialView("_AccountPartial", vm);
                }

                ModelState.Clear();

                TempData["msg"] = "<div class=\"col-sm-12 noPadding\"><h2 class=\"text-center greenCOLOR bold\">Successfully saved changes</h2></div>";

                return PartialView("_AccountPartial", vm);
            }
            else
            {
                return PartialView("_AccountPartial", vm);
            }


        }


    }
}