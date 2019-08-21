using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WoWScriptApi.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public ScriptItem ScriptItem { get; set; }
        public User Author { get; set; }
    }

    [NotMapped]
    public class CommentDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        //public int ScriptId { get; set; }
        public string AuthorName { get; set; }
    }
}
