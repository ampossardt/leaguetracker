﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace Engine.Helpers
{
    public class MailHelper
    {
        private readonly SmtpClient _client;
        private readonly string _templateLocation;

        public MailHelper(SmtpClient client)
        {
            _client = client;
            _templateLocation = ConfigurationManager.AppSettings["EmailTemplatePath"];
        }

        public bool SendRegistrationEmail(string email, string name, string authCode)
        {
            var body = new StringBuilder(File.ReadAllText(string.Format("{0}\\{1}", _templateLocation, "Confirm.htm")));
            body.Replace("##FirstName##", name);
            body.Replace("##ConfirmToken##", authCode);
            var url = HttpContext.Current.Request.Url;
            body.Replace("##Environment##", string.Format("http://{0}:{1}", url.Host, url.Port));

            try
            {
                _client.Send(new MailMessage("possardt514@gmail.com", email, "Confirm Your Account", body.ToString())
                {
                    IsBodyHtml = true
                });
            }
            catch (Exception ex)
            {
                //something went wrong
                return false;
            }

            return true;
        }

        public bool SendPasswordResetEmail(string email, string name, string resetCode)
        {
            StringBuilder body = new StringBuilder(File.ReadAllText(_templateLocation.Replace("MemorEaseWebApp", "Engine") + "\\Reset.htm"));
            body.Replace("##FirstName##", name);
            var url = HttpContext.Current.Request.Url;
            body.Replace("##Environment##", string.Format("http://{0}:{1}", url.Host, url.Port));
            body.Replace("##ConfirmToken##", resetCode);

            try
            {
                _client.Send(new MailMessage("possardt514@gmail.com", email, "Reset your password", body.ToString())
                {
                    IsBodyHtml = true
                });
            }
            catch (Exception ex)
            {
                //
                return false;
            }

            return true;
        }
    }
}