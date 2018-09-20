using AuthNetAPI;
using AuthorizeNet.Api.Contracts.V1;
using CashFlowOrganizer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CashFlowOrganizer.Controllers
{
    public class SignUpController : BaseController
    {
        public SignUpController()
        {
            db = new CashFlowOrganizerEntities();
        }

        public ActionResult Index()
        {
            NewUserViewModel user = new NewUserViewModel();
            //user.Billing = new Billing();

            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> Index(NewUserViewModel vm)
        {
            if (UserManager.FindByName(vm.Username ?? "") != null)
                ModelState.AddModelError("Username", "The username entered already exists, please enter a different username");

            if (UserManager.FindByEmail(vm.Email ?? "") != null)
                ModelState.AddModelError("Email", "The email entered already exists, please enter a different email");

            if (vm.Password != vm.ConfirmPassword)
                ModelState.AddModelError("ConfirmPassword", "The password field and confirm password field do not match.");

            var validatePassword = await UserManager.PasswordValidator.ValidateAsync(vm.Password ?? "");
            if (!validatePassword.Succeeded)
            {
                foreach (var error in validatePassword.Errors)
                {
                    ModelState.AddModelError("Password", error);
                }
            }

            AppUser user = new AppUser { UserName = vm.Username, Email = vm.Email };
            var validateUsername = await UserManager.UserValidator.ValidateAsync(user);
            if (!validateUsername.Succeeded)
            {
                foreach (var error in validateUsername.Errors)
                {
                    ModelState.AddModelError("Username", error);
                }
            }

            if (ModelState.IsValid)
            {
                /* CREATE USER */
                IdentityResult result = await UserManager.CreateAsync(user, vm.Password);
                var roleResult = await UserManager.AddToRoleAsync(user.Id, "REG");

                if (result.Succeeded)
                {
                    //string SubscriptionID;

                    //decimal amount = (decimal)4.95;

                    //creditCardType cct = new creditCardType();
                    //cct.cardNumber = vm.Billing.CardNumber;
                    //cct.expirationDate = vm.Billing.ExpirationMonth + vm.Billing.ExpirationYear;
                    //cct.cardCode = vm.Billing.SecurityCode;

                    //nameAndAddressType naat = new nameAndAddressType();
                    //naat.firstName = vm.FirstName;
                    //naat.lastName = vm.LastName;
                    //naat.address = vm.Billing.Address;
                    //naat.city = vm.Billing.City;
                    //naat.state = vm.Billing.State;
                    //naat.zip = vm.Billing.Zipcode;

                    //orderType order = new orderType();
                    //order.invoiceNumber = "";
                    //order.description = "Regular Subscription";

                    ///* CREATE SUBSCRIPTION IN AUTHORIZE.NET */
                    //ARBCreateSubscriptionResponse subResponse = Transactions.CreateSubscription.RunMonthlyAfter7DayFreeTrial(amount, cct, naat, order);

                    //if(subResponse.messages.resultCode == messageTypeEnum.Ok)
                    //{
                    //    SubscriptionID = subResponse.subscriptionId;                        

                        /* CREATE PROFILE */
                        var storedProcedure = db.ProfileInsert(user.Id, vm.FirstName, vm.LastName, "0").ToList();

                        if (storedProcedure.FirstOrDefault().ErrorNumber.HasValue == true)
                        {
                            /* VOID TRANSACTION, CANCEL SUBSCRIPTION, AND DELETE USER */
                            //Transactions.CancelSubscription.Run(SubscriptionID);
                            UserManager.Delete(user);
                            ModelState.AddModelError("RandomError", "Cannot insert profile. Please contact support.");
                            return View(vm);
                        }                            

                        /* SEND CONFIRMATION EMAIL TO NEW USER */
                        var emailResponse = SendConfirmEmail(user.Id, user.Email, user.UserName);

                        return View("ConfirmEmail");

                    //}
                    //else
                    //{
                    //    /* DELETE USER */
                    //    UserManager.Delete(user);
                    //    ModelState.AddModelError("Billing.CardNumber", subResponse.messages.message[0].text);
                    //    return View(vm);
                    //}                    
                }
                else
                {
                    foreach (string error in result.Errors)
                    {                        
                        ModelState.AddModelError("RandomError", error);
                    }
                }
            }

            return View(vm);
        }


      

    }
}