using CashFlowOrganizer.Infrastructure;
using CashFlowOrganizer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CashFlowOrganizer.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        public AccountController()
        {
            db = new CashFlowOrganizerEntities();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult SignIn(string message)
        {
            if(!string.IsNullOrEmpty(message))
                ModelState.AddModelError("Summary", message);

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignIn(string Username, string Password)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await UserManager.FindAsync(Username, Password);

                if (user == null)
                {
                    ModelState.AddModelError("Summary", "Invalid name or password");
                }
                else
                {
                    if (!user.EmailConfirmed)
                    {
                        ModelState.AddModelError("Summary", "You must first confirm your email address. Another confirmation email has been sent to you.");

                        /* SEND ANOTHER CONFIRMATION EMAIL TO NEW USER */
                        var emailResponse = SendConfirmEmail(user.Id, user.Email, user.UserName);

                        return View();
                    }
                    else
                    {
                        ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                        AuthManager.SignOut();
                        AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);
                        
                        var profileUser = db.ProfileSelectByUserID(user.Id).FirstOrDefault();
                        Session["ProfileID"] = profileUser.Value;
                        Session["UserID"] = user.Id;

                        if (await UserManager.IsInRoleAsync(user.Id, "ADM"))
                        {
                            return RedirectToAction("Index", "Settings", new { area = "Admin" });
                        }
                        else if (await UserManager.IsInRoleAsync(user.Id, "REG"))
                        {
                            return RedirectToAction("Index", "Dashboard", new { area = "Main" });
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }                    
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult SignOut(string message)
        {
            AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            string[] myCookies = Request.Cookies.AllKeys;
            foreach (string cookie in myCookies)
            {
                Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            }

            Session.Abandon();

            return RedirectToAction("SignIn", "Account", new { message =  message });
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("InvalidLink");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "InvalidLink");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ForgotPasswordViewModel vm = new ForgotPasswordViewModel();

            return View(vm);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel vm)
        {
            if(vm.Email != vm.ConfirmEmail)
            {
                ModelState.AddModelError("ConfirmEmail", "Confirm email must match the email above.");
            }

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(vm.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Summary", "Invalid email");
                    return View(vm);
                }
                else if(!(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    ModelState.AddModelError("Summary", "You must first confirm your email before you can reset your password. Another confirmation email has been sent to you.");

                    var emailResponse = SendConfirmEmail(user.Id, user.Email, user.UserName);

                    return View(vm);
                }
                else
                {
                    var emailResponse = SendResetPasswordEmail(user.Id, user.Email, user.UserName);

                    ModelState.AddModelError("Summary", "An email has been sent to you to reset your passord. You may login once your password has been reset.");

                    return View("SignIn");
                }
            }

            return View(vm);
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string userId, string code)
        {
            var user = UserManager.FindByIdAsync(userId).Result;

            if(user == null || code == null)
            {
                return View("InvalidLink");
            }

            Session["UserIdSession"] = userId;
            Session["CodeSession"] = code;

            ResetPasswordViewModel vm = new ResetPasswordViewModel();

            return View(vm);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            if(vm.Password != vm.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Confirm password must match the password above.");
            }

            if (ModelState.IsValid)
            {
                string userId = Session["UserIdSession"] as string;
                string code = Session["CodeSession"] as string;

                var user = await UserManager.FindByIdAsync(userId);
                if (user == null)
                {
                    ModelState.AddModelError("Summary", "Invalid link. Please try to reset your passord again.");

                    return View("SignIn");
                }
                else
                {
                    var result = await UserManager.ResetPasswordAsync(user.Id, code, vm.Password);

                    if (result.Succeeded)
                    {
                        ModelState.AddModelError("Summary", "Your password has been reset. You may now login with your new password.");

                        Session.Remove("UserIdSession");
                        Session.Remove("CodeSession");

                        return View("SignIn");
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
            }
            else
            {
                return View(vm);
            }
            
        }

        [AllowAnonymous]
        public ActionResult ForgotUsername()
        {
            ForgotPasswordViewModel vm = new ForgotPasswordViewModel();

            return View(vm);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotUsername(ForgotPasswordViewModel vm)
        {
            if (vm.Email != vm.ConfirmEmail)
            {
                ModelState.AddModelError("ConfirmEmail", "Confirm email must match the email above.");
            }

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(vm.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Summary", "Invalid email");
                    return View(vm);
                }
                else if (!(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    ModelState.AddModelError("Summary", "You must first confirm your email. Another confirmation email has been sent to you.");

                    var emailResponse = SendConfirmEmail(user.Id, user.Email, user.UserName);

                    return View(vm);
                }
                else
                {
                    var emailResponse = SendForgotUsernameEmail(user.Id, user.Email, user.UserName);

                    ModelState.AddModelError("Summary", "An email has been sent to you containing your username.");

                    return View("SignIn");
                }
            }

            return View(vm);
        }

    }
}