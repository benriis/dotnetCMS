using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WoWScriptApi.Models
{
    public class Likes
    {
        public int Id { get; set; }
        public ScriptItem ScriptItem { get; set; }
        public User Author { get; set; }
    }
}
