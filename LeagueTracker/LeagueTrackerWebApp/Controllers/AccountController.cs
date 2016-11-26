using System;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LeagueTrackerWebApp.Filters;
using LeagueTrackerWebApp.Models;
using WebMatrix.WebData;
using Engine.Extensions;
using Engine.Helpers;

namespace LeagueTrackerWebApp.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        private readonly MailHelper _mailHelper;

        public AccountController()
        {
            _mailHelper = new MailHelper(new SmtpClient()
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential("possardt514@gmail.com", "emfjuh12"),
                EnableSsl = true,
                Port = 587,
                Host = "smtp.gmail.com"
            });
        }
        
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
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
                        model.FirstName,
                        model.LastName,
                        model.Email
                    }, true);
                
                if (!string.IsNullOrEmpty(token))
                {
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

            return View("~/Views/Shared/Confirm.cshtml");
        }
    }
}
