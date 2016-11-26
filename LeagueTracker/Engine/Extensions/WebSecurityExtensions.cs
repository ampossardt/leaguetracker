using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using WebMatrix.Data;
using WebMatrix.WebData;

namespace Engine.Extensions
{
    public static class WebSecurityExtensions
    {
        public static string GetUsernameByEmail(string email)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                var username = conn.Query<string>("Select top 1 Username from users where Email = @Email",
                    new { Email = email }).FirstOrDefault();
                var approved = conn.Query<bool>("Select top 1 IsConfirmed from webpages_Membership where UserId = @id",
                    new { id = WebSecurity.GetUserId(username) }).FirstOrDefault();

                return approved ? username : string.Empty;
            }
        }

        public static bool EmailAlreadyExists(string email)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                try
                {
                    return conn.Query("Select email from users where email = @Email", new { Email = email }).Any();
                }
                catch (Exception ex)
                {
                    return true;
                }
            }
        }
    }
}