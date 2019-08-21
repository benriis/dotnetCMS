using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WoWScriptApi.Models
{
    public class ScriptItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ClassName { get; set; }
        public User Author { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }
        public ICollection<ImageItem> ImageItems { get; set; }
        public ICollection<Tags> Tags { get; set; }
        public ICollection<Likes> Likes { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }

    [NotMapped]
    public class ScriptItemDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ClassName { get; set; }
        public string Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ImageItem> ImageItems { get; set; }
        public ICollection<Tags> Tags { get; set; }
        public ICollection<string> StringTags { get; set; }
        public int Likes { get; set; }
        public ICollection<CommentDTO> Comments { get; set; }
    }
}
 