using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Engine.Data.Users.Data;
using Engine.Extensions;
using Engine.Helpers;
using Engine.Util;
using LeagueTrackerWebApp.Filters;
using LeagueTrackerWebApp.Models;
using WebMatrix.WebData;

namespace LeagueTrackerWebApp.Controllers
{
    [InitializeSimpleMembership]
    public class RegisterController : Controller
    {
        private readonly MailHelper _mailHelper;

        public RegisterController()
        {
            var username = ConfigSettings.Instance.GetConfigValue(ConfigSettings.MailServerUsername);
            var password = ConfigSettings.Instance.GetConfigValue(ConfigSettings.MailServerPassword);
            var server = ConfigSettings.Instance.GetConfigValue(ConfigSettings.MailServer);

            _mailHelper = new MailHelper(new SmtpClient()
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(username, password),
                EnableSsl = true,
                Port = 587,
                Host = server
            });
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegistrationModels.RegisterModel model)
        {
            if (Request.QueryString["delete"] == "y")
            {
                WebSecurityExtensions.DeleteUser(model.Username);
            }

            if (!ModelState.IsValid) return View(model);

            if (!WebSecurityExtensions.EmailAlreadyExists(model.Email))
            {
                ModelState.AddModelError("EmailTaken", "The email specified already exists. Please try registering again with a valid email address.");

                return View(model);
            }

            try
            {
                var token = WebSecurity.CreateUserAndAccount(model.Username, model.Password, 
                    new
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email
                    }, true);

                if (!string.IsNullOrEmpty(token))
                {
                    Roles.AddUserToRole(model.Username,
                        model.UserType == RegistrationModels.RegisterModel.OwnerValue
                            ? RegistrationModels.RegisterModel.OwnerValue
                            : RegistrationModels.RegisterModel.CoachValue);

                    _mailHelper.SendRegistrationEmail(model.Email, model.FirstName, token);
                }
            }
            catch (InvalidOperationException ex)
            {
                // log something here: unable to register
                return View(model);
            }
            catch (HttpException ex)
            {
                // couldnt set auth cookie
                return View(model);
            }

            ViewBag.Title = "Confirmation Sent";
            ViewBag.ConfirmationMessage = "Thanks for signing up. A confirmation message has been sent to your email address.";

            return View("~/Views/Shared/Confirmation.cshtml");
        }

        [AllowAnonymous]
        public ActionResult Confirm(string id)
        {
            ViewBag.Title = "Confirm Your Account";
            var successMessage = "Thanks for confirming your account! You will now be redirected to the login page.";
            var failureMessage = "Sorry, the account confirmation token you have provided is expired or invalid." +
                                  "Please <a href=\"/Register/ResendConfirmation\">click here</a> to send a new confirmation.";

            if (string.IsNullOrEmpty(id)) return Redirect("/");

            if (!WebSecurity.ConfirmAccount(id))
            {
                ViewBag.ConfirmationMessage = failureMessage;
                return View("~/Views/Shared/Confirmation.cshtml");
            }

            ViewBag.ConfirmationMessage = successMessage;
            ViewBag.Success = true;
            return View("~/Views/Shared/Confirmation.cshtml");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult DeleteUser()
        {
            var user = Request.Params["user"];

            if (!string.IsNullOrEmpty(user))
            {
                WebSecurityExtensions.DeleteUser(user);
            }

            return Redirect("/");
        }
    }
}
