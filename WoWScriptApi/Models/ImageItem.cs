using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WoWScriptApi.Models
{
    public class ImageItem
    {
        public int Id { get; set; }
        public string ImageId { get; set; }
        public int ScriptId { get; set; }
        public ScriptItem ScriptItem { get; set; }
    }
}
