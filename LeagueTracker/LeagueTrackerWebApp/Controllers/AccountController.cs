using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;
using Engine.Data.Types;
using Engine.Data.Users;
using Engine.Extensions;
using LeagueTrackerWebApp.Filters;
using LeagueTrackerWebApp.Models;
using WebMatrix.WebData;
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
            if (!ModelState.IsValid) return View(model);

            if (!WebSecurity.Login(model.UserName, model.Password, model.RememberMe))
            {
                return View(model);
            }

            return !string.IsNullOrEmpty(returnUrl) ? Redirect(returnUrl) : Redirect("/Account/Home");
        }

        public ActionResult Home()
        {
            if (!WebSecurity.IsAuthenticated) return Redirect("/");

            var roles = Roles.GetRolesForUser(WebSecurity.CurrentUserName);

            ILeagueUser currentUser;

            if (roles.Contains(RegistrationModels.RegisterModel.OwnerValue))
            {
                currentUser = new Owner(WebSecurity.CurrentUserId);
            }
            else
            {
                currentUser = new Coach(WebSecurity.CurrentUserId);
            }
            
            // Hmm, didn't really think about the fact that either a league owner or a 
            // coach can go to this page. so we really need to know what kind of user this
            // is and also figure out a way to render the page differently dependant on that.
            return View(currentUser);
        }

        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetModel model)
        {
            if (!ModelState.IsValid) return View(model);

            return new AccountHelper().SendResetEmail(model.Email) 
                ? View(model) : View("~/Views/Shared/Confirmation.cshtml");
        }
    }
}
