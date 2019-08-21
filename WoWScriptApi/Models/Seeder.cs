using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WoWScriptApi.Models
{
    public static class Seeder
    {
        public static void SeedDb(ScriptContext context)
        {
            context.ScriptItems.RemoveRange(context.ScriptItems);
            context.Users.RemoveRange(context.Users);
            context.Likes.RemoveRange(context.Likes);

            var itemOne = new ScriptItem
            {
                Title = "Hans' Hunter string",
                Content = "From Seeder",
                ClassName = "Hunter",
                Author = new User
                {
                    Username = "hans",
                    Password = "AQAAAAEAACcQAAAAEOnBtGXVwjjNe9NOlVMRVNbUvAfQWJhvd3T/zQ3NCATJY+QQeajFP6vDgqmZEj3qcA=="
                },
                CreatedAt = new DateTime(2018, 7, 15)
            };

            var itemTwo = new ScriptItem
            {
                Title = "Second",
                Content = "From Seeder",
                ClassName = "Paladin",
                Author = new User
                {
                    Username = "Jens",
                    Password = "AQAAAAEAACcQAAAAEOnBtGXVwjjNe9NOlVMRVNbUvAfQWJhvd3T/zQ3NCATJY+QQeajFP6vDgqmZEj3qcA=="
                },
                CreatedAt = DateTime.Now
            };


            context.AddRange(itemOne, itemTwo);

            var tagsOne = new Tags[]
            {
                new Tags
                {
                    Spec = "Ret",
                    ScriptItem = itemOne
                },
                new Tags
                {
                    Spec = "Holy",
                    ScriptItem = itemOne
                }
            };

            var tagsTwo = new Tags[]
            {
                new Tags
                {
                    Spec = "Arms",
                    ScriptItem = itemTwo
                },
                new Tags
                {
                    Spec = "Prot",
                    ScriptItem = itemTwo
                }
            };

            context.AddRange(tagsOne);
            context.AddRange(tagsTwo);



            context.SaveChanges();
            System.Diagnostics.Debug.WriteLine("LISTY HERE");
            System.Diagnostics.Debug.WriteLine(context.ScriptItems);

        }

        //private readonly ScriptContext _context;
        //public Seeder(ScriptContext context)
        //{
        //    _context = context;
        //}

        //public static void SeedData()
        //{
        //    AddNewType(new ScriptItem { Title = "Seeder", Content = "From Seeder", ClassName = "Seed", Author = new User { Username = "hans", Password = "123" }, CreatedAt = DateTime.Now });
        //    _context.SaveChanges();
        //}

        //public static void AddNewType(ScriptItem scriptItem)
        //{
        //    var existingType = _context.ScriptItems.FirstOrDefault(p => p.Title == scriptItem.Title);
        //    if (existingType == null)
        //    {
        //        _context.ScriptItems.Add(scriptItem);
        //    }
        //}
    }
}
