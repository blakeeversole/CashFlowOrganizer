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
        public SettingsController()
        {
            db = new CashFlowOrganizerEntities();
        }

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

        [HttpGet]
        public ActionResult ManageUsers()
        {
            ManageUsersViewModel vm = new ManageUsersViewModel();

            vm.UserList = new List<UserFields>();

            var result = db.AspNetUsersSelect().ToList();

            foreach (var item in result)
            {
                UserFields user = new UserFields();

                user.AspNetUsersID = item.AspNetUsersID;
                user.FirstName = item.FirstName;
                user.LastName = item.LastName;
                user.Role = item.RoleDescr;

                vm.UserList.Add(user);
            }

            return View(vm);
        }

        [HttpGet]
        public ActionResult EditUser(string AspNetUsersID)
        {
            UserEdit vm = new UserEdit();

            var person = db.AspNetUsersSelectByAspNetUsersID(AspNetUsersID).FirstOrDefault();

            vm.AspNetUsersID = person.AspNetUsersID;
            vm.FirstName = person.FirstName;
            vm.LastName = person.LastName;
            vm.Role = person.RoleId;

            ViewBag.Roles = db.RoleSelectAll();


            return View(vm);
        }

        [HttpPost]
        public ActionResult EditUser(UserEdit vm)
        {
            var result = db.AspNetUserUpdate(vm.AspNetUsersID,vm.FirstName,vm.LastName,vm.Role);

            return RedirectToAction("ManageUsers");
        }

    }
}