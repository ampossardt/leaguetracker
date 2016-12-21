using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using Dapper;
using Engine.Data.Users;
using Engine.Data.Users.Data;
using Engine.Util;
using WebMatrix.Data;
using WebMatrix.WebData;

namespace Engine.Extensions
{
    public static class WebSecurityExtensions
    {
        private static readonly string _connectionString =
            ConfigSettings.Instance.GetConnectionString(ConfigSettings.DefaultConnectionString);

        public static Owner GetOwner(int userId)
        {
            var owner = new Owner
            {
                Profile = GetUserProfile(userId),
                AccountInformation = GetUserStatistics(userId)
            };

            return owner;
        }

        private static UserProfile GetUserProfile(int userId)
        {
            UserProfile profile;

            using (var conn = new SqlConnection(_connectionString))
            {
                profile = conn.QueryFirst<UserProfile>("select * from UserProfile where @UserId = UserId",
                    new {UserId = userId});
            }

            return profile ?? new UserProfile();
        }

        private static UserStatistics GetUserStatistics(int userId)
        {
            UserStatistics statistics;

            using (var conn = new SqlConnection(_connectionString))
            {
                statistics = conn.QueryFirst<UserStatistics>("select * from webpages_Membership where @UserId = UserId",
                    new { UserId = userId });
            }

            return statistics ?? new UserStatistics();
        }

        public static UserProfile GetUserProfileByEmail(string email)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var profile = conn.QueryFirst<UserProfile>("Select * from UserProfile where @Email = Email",
                    new {Email = email});

                return profile ?? new UserProfile();
            }
        }

        public static string GetUsernameByEmail(string email)
        {
            using (var conn = new SqlConnection(_connectionString))
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
            using (var conn = new SqlConnection(_connectionString))
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

        public static void DeleteUser(string username)
        {
            var provider = (SimpleMembershipProvider) Membership.Provider;

            RemoveUserRoles(username);
            provider.DeleteAccount(username);
            provider.DeleteUser(username, true);
        }

        private static void RemoveUserRoles(string username)
        {
            var roles = Roles.GetRolesForUser(username);

            if (roles != null && roles.Length > 0)
            {
                Roles.RemoveUserFromRoles(username, roles);
            }
        }
    }
}