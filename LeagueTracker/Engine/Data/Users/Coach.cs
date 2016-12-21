using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Engine.Data.Users
{
    public class Coach : ILeagueUser
    {
        public Coach()
        {   
        }

        public Coach(int userId)
        {
        }

        public string Name { get; set; }
        public string fuckoff { get; set; }
    }
}