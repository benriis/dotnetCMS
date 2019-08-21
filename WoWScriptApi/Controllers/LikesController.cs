using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WoWScriptApi.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WoWScriptApi.Controllers
{
    [Route("api/scripts/{scriptId}/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly ScriptContext _context;

        public LikesController(ScriptContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Index(int scriptId)
        {
            return Ok(new {msg = "hey from likes " + scriptId});
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<int>> PostLikes(int scriptId)
        {
            

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var author = await _context.Users.FindAsync(userId);
            var scriptItem = await _context.ScriptItems.FindAsync(scriptId);
            var check = _context.Likes
                .Include(x => x.ScriptItem)
                .Include(x => x.Author)
                .SingleOrDefault(x => x.Author.Id == userId && x.ScriptItem.Id == scriptId);

            if (check == null)
            {
                AddLike(scriptItem, author);
            } else
            {
                RemoveLike(check);
            }

            await _context.SaveChangesAsync();

            var returnObject = _context.Likes
                .Include(x => x.ScriptItem)
                .Count(x => x.ScriptItem.Id == scriptId);

            return returnObject;
        }

        private void AddLike(ScriptItem scriptItem, User author)
        {
            var like = new Likes
            {
                ScriptItem = scriptItem,
                Author = author
            };

            _context.Likes.Add(like);
        }

        private void RemoveLike(Likes like)
        {
            _context.Likes.Remove(like);
        }
    }
}
