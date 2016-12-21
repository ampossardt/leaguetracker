using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Engine.Data.Users.Data;
using Engine.Extensions;

namespace Engine.Data.Users
{
    public class Owner : ILeagueUser
    {
        public Owner()
        {    
        }

        public Owner(int userId)
        {
            var currentUser = WebSecurityExtensions.GetOwner(userId);
            Profile = currentUser.Profile;
            AccountInformation = currentUser.AccountInformation;
        }

        public string Name { get; set; }
        public UserProfile Profile { get; set; }
        public UserStatistics AccountInformation { get; set; }
    }
}