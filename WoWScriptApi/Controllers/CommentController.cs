using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WoWScriptApi.Models;

namespace WoWScriptApi.Controllers
{
    [Route("api/scripts/{scriptId}/[controller]")]
    [Authorize]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ScriptContext _context;

        public CommentController(ScriptContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] ContentModel content, int scriptId)
        {
            var author = await GetUserAsync();
            var scriptItem = await _context.ScriptItems.FindAsync(scriptId);

            Comment comment = new Comment
            {
                Content = content.Content,
                Author = author,
                ScriptItem = scriptItem
            };

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return Ok();

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditComment([FromBody] ContentModel content, int id)
        {
            var author = await GetUserAsync();
            var commentItem = await _context.Comments
                .Include(x => x.Author)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (commentItem == null) { return NotFound(); };
            if (commentItem.Author != author) { return Unauthorized(); };
            if (commentItem.Author == author)
            {
                commentItem.Content = content.Content;
                await _context.SaveChangesAsync();
                return Ok(commentItem);
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var author = await GetUserAsync();
            var comment = await _context.Comments
                .Include(x => x.Author)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            //Debug.WriteLine("============================");
            //Debug.WriteLine(comment);
            //Debug.WriteLine("============================");
            Debug.WriteLine("============================");
            Debug.WriteLine(author.Username);
            Debug.WriteLine(comment.Author.Username);
            Debug.WriteLine("============================");

            if (comment == null) {
                Debug.WriteLine("Niggershit");

                return BadRequest();
            };
            if (comment.Author.Id == author.Id)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                return Ok();
            }

            return BadRequest();
        }


        public class ContentModel
        {
            public string Content { get; set; }
        }

        private async Task<User> GetUserAsync()
        {
            var authorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(authorId);


            return user;
        }
    }

}
