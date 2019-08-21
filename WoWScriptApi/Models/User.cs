using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WoWScriptApi.Models
{
    public class User
    {
        public int Id { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public static implicit operator User(ClaimsPrincipal v)
        {
            throw new NotImplementedException();
        }
        //public string Token { get; set; }

    }
}
