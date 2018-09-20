using CashFlowOrganizer.Areas.Admin.Models;
using CashFlowOrganizer.Controllers;
using CashFlowOrganizer.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CashFlowOrganizer.Areas.Admin.Controllers
{
    [Authorize(Roles = "ADM")]
    public class SettingsController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddRole()
        {
            RoleViewModel vm = new RoleViewModel();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddRole(RoleViewModel vm)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await RoleManager.CreateAsync(new AppRole() { Name = vm.Code, Descr = vm.Descr, SortOrder = vm.SortOrder });

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("Summary", error);
                    }

                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }
    }
}