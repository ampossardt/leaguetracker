using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using Engine.Data.Users.Data;
using Engine.Extensions;
using Engine.Util;
using WebMatrix.WebData;

namespace Engine.Helpers
{
    public class RegistrationHelper
    {
        private readonly string _connectionString = 
            ConfigSettings.Instance.GetConnectionString(ConfigSettings.DefaultConnectionString);

        public RegistrationHelper()
        {       
        }

        /// <summary>
        ///  Adds associated profile for this user. This is done automatically through WebSecurity.CreateUserAndAccount
        /// </summary>
        /// <param name="profile">User's profile info to be stored.</param>
        /// <returns></returns>
        [Obsolete]
        public bool CreateNewProfile(UserProfile profile)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                const string query = "SET IDENTITY_INSERT dbo.UserProfile ON" +
                                     "insert into dbo.UserProfile values(@UserId, @Username, @FirstName, @LastName, @Email)" +
                                     "SET IDENTITY_INSERT dbo.UserProfile OFF";

                try
                {
                    conn.Execute(query, profile);
                }
                catch
                {
                    // oopsies
                    WebSecurityExtensions.DeleteUser(profile.Username);
                    return false;
                }
            }

            return true;
        }
    }
}