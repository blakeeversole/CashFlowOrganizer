using CashFlowOrganizer.Infrastructure;
using CashFlowOrganizer.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CashFlowOrganizer.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            //SET CULTURE
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo newCulture = new CultureInfo(currentCulture.Name);
            newCulture.NumberFormat.CurrencyNegativePattern = 1;
            Thread.CurrentThread.CurrentCulture = newCulture;
        }

        public CashFlowOrganizerEntities db;
        public IAuthenticationManager AuthManager { get { return HttpContext.GetOwinContext().Authentication; } }
        public AppUserManager UserManager { get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); } }
        public AppRoleManager RoleManager { get{return HttpContext.GetOwinContext().GetUserManager<AppRoleManager>();}}

        public async Task<Response> SendConfirmEmail(string userId, string userEmail, string username)
        {
            string callBackUrl = CreateConfirmEmailCallBackUrl(userId);

            var client = new SendGridClient(ConfigurationManager.AppSettings["sgApiKey"]);
            var from = new EmailAddress("noreply@cashfloworganizer.com", "Cash Flow Organizer");
            var subject = "Confirm your account";
            var to = new EmailAddress(userEmail, username);
            var plainTextContent = "Please confirm your account by clicking <a href=\"" + callBackUrl + "\">here</a>";
            var htmlContent = "<strong>Please confirm your account by clicking <a href=\"" + callBackUrl + "\">here</a></strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            return response;
        }

        public async Task<Response> SendResetPasswordEmail(string userId, string userEmail, string username)
        {
            string callBackUrl = CreateResetPasswordCallBackUrl(userId);

            var client = new SendGridClient(ConfigurationManager.AppSettings["sgApiKey"]);
            var from = new EmailAddress("noreply@cashfloworganizer.com", "Cash Flow Organizer");
            var subject = "Reset Password";
            var to = new EmailAddress(userEmail, username);
            var plainTextContent = "Please reset your password by clicking <a href=\"" + callBackUrl + "\">here</a>";
            var htmlContent = "<strong>Please reset your password by clicking <a href=\"" + callBackUrl + "\">here</a></strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            return response;
        }

        public async Task<Response> SendForgotUsernameEmail(string userId, string userEmail, string username)
        {
            var client = new SendGridClient(ConfigurationManager.AppSettings["sgApiKey"]);
            var from = new EmailAddress("noreply@cashfloworganizer.com", "Cash Flow Organizer");
            var subject = "Forgot Username";
            var to = new EmailAddress(userEmail, username);
            var plainTextContent = "Your username is: " + username;
            var htmlContent = "<strong>Your username is: " + username + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            return response;
        }

        public async Task<Response> SendCancelAccountEmail(string userId, string userEmail, string username)
        {
            var client = new SendGridClient(ConfigurationManager.AppSettings["sgApiKey"]);
            var from = new EmailAddress("noreply@cashfloworganizer.com", "Cash Flow Organizer");
            var subject = "Account Cancelled";
            var to = new EmailAddress(userEmail, username);
            var plainTextContent = "Your account has been cancelled. If you have any questions please contact customer support.";
            var htmlContent = "<strong>Your account has been cancelled. If you have any questions please contact customer support.</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            return response;
        }

        public string CreateConfirmEmailCallBackUrl(string userId)
        {
            string code = UserManager.GenerateEmailConfirmationTokenAsync(userId).Result;
            string callbackUrl = Url.Action("ConfirmEmail", "Account", new { area = "", userId = userId, code = code }, protocol: Request.Url.Scheme);

            return callbackUrl;
        }

        public string CreateResetPasswordCallBackUrl(string userId)
        {
            string code = UserManager.GeneratePasswordResetTokenAsync(userId).Result;
            var callbackUrl = Url.Action("ResetPassword", "Account", new { area = "", userId = userId, code = code }, protocol: Request.Url.Scheme);

            return callbackUrl;
        }

    }
}
