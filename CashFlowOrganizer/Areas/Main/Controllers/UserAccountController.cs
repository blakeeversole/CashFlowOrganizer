using AuthNetAPI;
using AuthorizeNet.Api.Contracts.V1;
using CashFlowOrganizer.Areas.Main.Models;
using CashFlowOrganizer.Controllers;
using CashFlowOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CashFlowOrganizer.Areas.Main.Controllers
{
    [Authorize(Roles = "REG")]
    public class UserAccountController : BaseController
    {
        public UserAccountController()
        {
            db = new CashFlowOrganizerEntities();
        }

        public ActionResult Index()
        {
            var profileId = Session["ProfileID"];
            var userId = Session["UserID"];

            if (profileId == null || userId == null)
                return RedirectToAction("SignOut", "Account", new { area = "" });

            UserAccount vm = new UserAccount();
            //vm.Billing = new Billing();

            var profile = db.ProfileSelect(Convert.ToInt32(profileId)).FirstOrDefault();

            //Session["SubscriptionID"] = profile.SubscriptionID;

            vm.FirstName = profile.FirstName;
            vm.LastName = profile.LastName;
            vm.Email = profile.Email;
            vm.Username = profile.UserName;

            //var subResponse = Transactions.GetSubscription.Run(profile.SubscriptionID);

            //vm.SubscriptionStatus = subResponse.subscription.status.ToString();

            //vm.Billing.Address = subResponse.subscription.profile.paymentProfile.billTo.address;
            //vm.Billing.City = subResponse.subscription.profile.paymentProfile.billTo.city;
            //vm.Billing.State = subResponse.subscription.profile.paymentProfile.billTo.state;
            //vm.Billing.Zipcode = subResponse.subscription.profile.paymentProfile.billTo.zip;

            return View(vm);
        }

        [HttpPost]
        public ActionResult UpdateEmail(string email)
        {          
            if (ModelState.IsValid)
            {
                var userId = Session["UserID"];

                if (userId == null)
                    return RedirectToAction("SignOut", "Account", new { area = "" });

                AppUser user = new AppUser();

                user = UserManager.FindByIdAsync((string)userId).Result;

                if(user.Email == email)
                {
                    return PartialView("_EmailPartial", email);
                }
                else
                {
                    user.Email = email;
                    user.EmailConfirmed = false;

                    var update = UserManager.UpdateAsync(user).Result;

                    if (update.Succeeded)
                    {
                        TempData["EmailMSG"] = "<div class=\"col-sm-12 noPadding\"><h4 class=\"text-center greenCOLOR bold\">Successfully saved changes</h4><p class=\"text-center font110\">You must confirm your email address before you will be able to sign in again.</p></div>";

                        var emailResponse = SendConfirmEmail(user.Id, user.Email, user.UserName);

                        return PartialView("_EmailPartial", email);
                    }
                    else
                    {
                        foreach (var item in update.Errors)
                        {
                            ModelState.AddModelError("Summary", item);
                        }

                        TempData["EmailMSG"] = "";

                        return PartialView("_EmailPartial", email);
                    }
                }              
            }
            else
            {
                return PartialView("_EmailPartial", email);
            }
        }

        [HttpPost]
        public ActionResult UpdateUsername(string username)
        {
            if (ModelState.IsValid)
            {
                var userId = Session["UserID"];

                if (userId == null)
                    return RedirectToAction("SignOut", "Account", new { area = "" });

                AppUser user = new AppUser();

                user = UserManager.FindByIdAsync((string)userId).Result;

                if (user.UserName == username)
                {
                    return PartialView("_UsernamePartial", username);
                }
                else
                {
                    user.UserName = username;

                    var update = UserManager.UpdateAsync(user).Result;

                    if (update.Succeeded)
                    {
                        TempData["UsernameMSG"] = "<div class=\"col-sm-12 noPadding\"><h4 class=\"text-center greenCOLOR bold\">Successfully saved changes</h4></div>";

                        return PartialView("_UsernamePartial", username);
                    }
                    else
                    {
                        foreach (var item in update.Errors)
                        {
                            ModelState.AddModelError("Summary", item);
                        }

                        TempData["UsernameMSG"] = "";

                        return PartialView("_UsernamePartial", username);
                    }
                }
            }
            else
            {
                return PartialView("_UsernamePartial", username);
            }
        }

        [HttpPost]
        public ActionResult UpdatePassword(UserAccount vm)
        {
            ModelState.Remove("Email");
            ModelState.Remove("Username");
            ModelState.Remove("FirstName");
            ModelState.Remove("LastName");
            ModelState.Remove("TermsAndConditions");
            ModelState.Remove("Billing.Address");
            ModelState.Remove("Billing.City");
            ModelState.Remove("Billing.State");
            ModelState.Remove("Billing.Zipcode");
            ModelState.Remove("Billing.CardNumber");
            ModelState.Remove("Billing.ExpirationMonth");
            ModelState.Remove("Billing.ExpirationYear"); 
            ModelState.Remove("Billing.SecurityCode");

            if (vm.Password != vm.ConfirmPassword)
                ModelState.AddModelError("Summary", "New Password and New Password Confirmation do not match.");

            if (ModelState.IsValid)
            {
                var userId = Session["UserID"];

                if (userId == null)
                    return RedirectToAction("SignOut", "Account", new { area = "" });

                var update = UserManager.ChangePasswordAsync((string)userId, vm.CurrentPassword, vm.Password).Result;

                if (update.Succeeded)
                {
                    TempData["PasswordMSG"] = "<div class=\"col-sm-12 noPadding\"><h4 class=\"text-center greenCOLOR bold\">Successfully saved changes</h4></div>";

                    return PartialView("_PasswordPartial", vm);
                }
                else
                {
                    foreach (var item in update.Errors)
                    {
                        ModelState.AddModelError("Summary", item);
                    }

                    TempData["PasswordMSG"] = "";

                    return PartialView("_PasswordPartial", vm);
                }                
            }
            else
            {
                return PartialView("_PasswordPartial", vm);
            }
        }

        //[HttpPost]
        //public ActionResult UpdateBilling(UserAccount vm)
        //{
        //    ModelState.Remove("CurrentPassword");
        //    ModelState.Remove("Password");
        //    ModelState.Remove("ConfirmPassword");
        //    ModelState.Remove("Email");
        //    ModelState.Remove("Username");

        //    if (ModelState.IsValid)
        //    {
        //        var SubscriptionID = Session["SubscriptionID"];

        //        if (SubscriptionID == null)
        //            return RedirectToAction("SignOut", "Account", new { area = "" });

        //        creditCardType cct = new creditCardType();
        //        cct.cardNumber = vm.Billing.CardNumber;
        //        cct.expirationDate = vm.Billing.ExpirationMonth + vm.Billing.ExpirationYear;
        //        cct.cardCode = vm.Billing.SecurityCode;

        //        nameAndAddressType naat = new nameAndAddressType();
        //        naat.firstName = vm.FirstName;
        //        naat.lastName = vm.LastName;
        //        naat.address = vm.Billing.Address;
        //        naat.city = vm.Billing.City;
        //        naat.state = vm.Billing.State;
        //        naat.zip = vm.Billing.Zipcode;

        //        ARBUpdateSubscriptionResponse response = Transactions.UpdateSubscription.Run((string)SubscriptionID, cct, naat);

        //        if(response.messages.resultCode == messageTypeEnum.Ok)
        //        {
        //            var profileId = Session["ProfileID"];

        //            if (profileId == null)
        //                return RedirectToAction("SignOut", "Account", new { area = "" });

        //            var result = db.ProfileUpdate(Convert.ToInt32(profileId), vm.FirstName, vm.LastName);

        //            TempData["BillingMSG"] = "<div class=\"col-sm-12 noPadding\"><h2 class=\"text-center greenCOLOR bold\">Successfully saved changes</h2></div>";

        //            return PartialView("_BillingPartial", vm);
        //        }
        //        else
        //        {
        //            TempData["BillingMSG"] = "";

        //            ModelState.AddModelError("Summary", response.messages.message[0].text);

        //            return PartialView("_BillingPartial", vm);
        //        }                
        //    }
        //    else
        //    {
        //        return PartialView("_BillingPartial", vm);
        //    }
        //}

        [HttpGet]
        public ActionResult CancelSubscription()
        {
            var profileId = Session["ProfileID"];
            var userId = Session["UserID"];
            //var subscriptionId = Session["SubscriptionID"];

            if (profileId == null || userId == null) //|| subscriptionId == null
                return RedirectToAction("SignOut", "Account", new { area = "", message = "Unable to cancel account. Please contact support." });

            //ARBCancelSubscriptionResponse response = Transactions.CancelSubscription.Run((string)subscriptionId);

            //if(response.messages.resultCode == messageTypeEnum.Ok)
            //{
                AppUser user = UserManager.FindByIdAsync((string)userId).Result;

                var removeUser = UserManager.DeleteAsync(user).Result;
                var removeProfile = db.ProfileDeleteALL((int)profileId);

                if (removeUser.Succeeded)
                {
                    var emailResponse = SendCancelAccountEmail(user.Id, user.Email, user.UserName);

                    return RedirectToAction("SignOut", "Account", new { area = "", message = "Successfully cancelled account" });
                }
                else
                {
                    return RedirectToAction("SignOut", "Account", new { area = "", message = "Unable to cancel account. Please contact support." });
                }
            //}
            //else
            //{
            //    return RedirectToAction("SignOut", "Account", new { area = "", message = "Unable to cancel account. Please contact support." });
            //}            
        }
    }
}