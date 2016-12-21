using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Data.Users
{
    public interface ILeagueUser
    {
        string Name { get; set; }
    }
}
