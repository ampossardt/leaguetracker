using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Engine.Util
{
    public class ConfigSettings
    {
        private static ConfigSettings _instance;

        private ConfigSettings()
        {
            
        }

        public static ConfigSettings Instance
        {
            get
            {
                return _instance ?? (_instance = new ConfigSettings());
            }
        }

        public string GetConfigValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public string GetConnectionString(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ToString();
        }

        public const string MailServerUsername = "MailServerUsername";
        public const string MailServerPassword = "MailServerPassword";
        public const string MailServer = "MailServer";
        public const string DefaultConnectionString = "DefaultConnection";
    }
}