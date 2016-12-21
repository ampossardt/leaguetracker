using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Engine.Data.Users.Data;
using Engine.Extensions;
using Engine.Util;
using WebMatrix.WebData;

namespace Engine.Helpers
{
    public class AccountHelper
    {
        private readonly MailHelper _mailHelper;

        public AccountHelper()
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

        public bool SendResetEmail(string email)
        {
            var profile = WebSecurityExtensions.GetUserProfileByEmail(email);
            var token = GetPasswordResetToken(email);

            if(string.IsNullOrEmpty(profile.Email) || string.IsNullOrEmpty(token))
            {
                return false;
            }

            return _mailHelper.SendPasswordResetEmail(profile.Email, profile.FirstName, token);
        }

        private string GetPasswordResetToken(string email)
        {
            var username = WebSecurityExtensions.GetUsernameByEmail(email);

            return WebSecurity.GeneratePasswordResetToken(username);
        }
    }
}