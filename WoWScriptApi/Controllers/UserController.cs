using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WoWScriptApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WoWScriptApi.Controllers
{
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly ScriptContext _context;

        public UserController(ScriptContext context)
        {
            _context = context;
        }

        // GET: api/<controller>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScriptItem>>> GetTodoItems()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var author = await _context.Users.FindAsync(int.Parse(userId));
            return await _context.ScriptItems.Include(item => item.ImageItems).Where(item => item.Author == author).ToListAsync();
        }
    }
}
