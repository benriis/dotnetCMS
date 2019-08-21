using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WoWScriptApi.Models
{
    public class ScriptContext : DbContext
    {
        public ScriptContext(DbContextOptions<ScriptContext> options) : base(options)
        {

        }

        public DbSet<ScriptItem> ScriptItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ImageItem> ImageItem { get; set; }
        public DbSet<Tags>  Tags { get; set; }  
        public DbSet<Likes> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
