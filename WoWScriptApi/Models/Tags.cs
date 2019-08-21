using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WoWScriptApi.Models
{
    public class Tags
    {
        public int Id { get; set; }
        public string Spec { get; set; }
        public int ScriptId { get; set; }
        public ScriptItem ScriptItem { get; set; }
    }
}
