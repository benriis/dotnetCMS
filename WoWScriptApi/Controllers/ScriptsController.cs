using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WoWScriptApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WoWScriptApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScriptsController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingInviroment;
        private readonly ScriptContext _context;

        public ScriptsController(ScriptContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingInviroment = hostingEnvironment;
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScriptItemDTO>>> GetScriptItems([FromQuery(Name = "class")] string c, [FromQuery(Name = "sort")] string s)
        {

            var query = _context.ScriptItems.Select(x => new ScriptItemDTO
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                ClassName = x.ClassName,
                CreatedAt = x.CreatedAt,
                Author = x.Author.Username,
                ImageItems = x.ImageItems,
                StringTags = x.Tags.Select(y => y.Spec).ToList(),
                Likes = x.Likes.Count,
                Comments = x.Comments.Select(y => new CommentDTO {
                    Id = y.Id,
                    Content = y.Content,
                    AuthorName = y.Author.Username
                }).ToList()
            });

            if (c != null)
            {
                query = query.Where(x => x.ClassName == c);
            }

            switch (s)
            {
                case "asc":
                    query = query.OrderBy(x => x.CreatedAt);
                    break;
                case "desc":
                    query = query.OrderByDescending(x => x.CreatedAt);
                    break;
                default:
                    break;
            }

            if (s != null)
            {
                query = query.OrderByDescending(x => x.CreatedAt);
            }



            return await query.ToListAsync();

        }

        // GET: api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ScriptItemDTO>> GetScriptItem(int id)
        {
            var scriptItem = await _context.ScriptItems.Select(x => new ScriptItemDTO
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                ClassName = x.ClassName,
                CreatedAt = x.CreatedAt,
                Author = x.Author.Username,
                ImageItems = x.ImageItems,
                StringTags = x.Tags.Select(y => y.Spec).ToList()
            }).Where(x => x.Id == id)
            .ToListAsync();

            if (!scriptItem.Any())
            {
                return NotFound();
            }

            return scriptItem.First();
        }

        // POST api/<controller>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ScriptItem>> PostScriptItem([FromBody] ScriptItemDTO item)
        {

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var Author = await _context.Users.FindAsync(userId);

            ScriptItem scriptItem = new ScriptItem
            {
                Title = item.Title,
                Content = item.Content,
                ClassName = item.ClassName,
                Author = Author,
                CreatedAt = DateTime.Now
            };

            _context.ScriptItems.Add(scriptItem);

            foreach (var strings in item.StringTags)
            {
                Tags tag = new Tags
                {
                    Spec = strings,
                    ScriptItem = scriptItem
                };

                _context.Tags.Add(tag);
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetScriptItem), new { id = scriptItem.Id }, scriptItem);
        }

        // PUT api/<controller>/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutScriptItem(int id, [FromBody] ScriptItemDTO item)
        {

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var Author = await _context.Users.FindAsync(userId);

            var scriptItem = await _context.ScriptItems
                .Include(x => x.Author)
                .Include(x => x.Tags)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (scriptItem == null) { return StatusCode(404); };
            if (scriptItem.Author != Author) { return StatusCode(403); };

            var tagItems = await _context.Tags.Where(x => x.ScriptItem == scriptItem).ToListAsync();
            _context.Tags.RemoveRange(tagItems.ToList());

            _context.ScriptItems.Attach(scriptItem);

            scriptItem.Title = item.Title;
            scriptItem.Content = item.Content;
            scriptItem.ClassName = item.ClassName;

            if (item.StringTags != null)
            {
                foreach (var tags in item.StringTags)
                {
                    Tags tag = new Tags
                    {
                        Spec = tags,
                        ScriptItem = scriptItem
                    };

                    _context.Tags.Add(tag);
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScriptItem(int id)
        {
            var scriptItem = await _context.ScriptItems.FindAsync(id);
            var tagItems = await _context.Tags.Where(x => x.ScriptItem == scriptItem).ToListAsync();
            var imageItems = await _context.ImageItem.Where(x => x.ScriptItem == scriptItem).ToListAsync();
            var Comments = await _context.Comments.Where(x => x.ScriptItem == scriptItem).ToListAsync();

            if (scriptItem == null)
            {
                return NotFound();
            }

            _context.Comments.RemoveRange(Comments);
            _context.ImageItem.RemoveRange(imageItems);
            _context.Tags.RemoveRange(tagItems);
            _context.ScriptItems.Remove(scriptItem);
            
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("upload/{id}")]
        public async Task<ActionResult<ImageItem>> Post(List<IFormFile> files, int id)
        {
            long size = files.Sum(f => f.Length);
            var filePath = Path.Combine(_hostingInviroment.ContentRootPath, "Images");
            // Check if int scriptId is even used
            int scriptId = id;
            var scriptItem = await _context.ScriptItems.FindAsync(id);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    string uniqueFilename = Guid.NewGuid().ToString();
                    string fileExtention = formFile.FileName.Split(".").Last();
                    string filename = uniqueFilename + "." + fileExtention;

                    using (var stream = new FileStream(Path.Combine(filePath, filename), FileMode.Create))
                    {

                        await formFile.CopyToAsync(stream);
                    }

                    ImageItem newImage = new ImageItem
                    {
                        ImageId = filename,
                        ScriptId = id,
                        ScriptItem = scriptItem
                    };

                    _context.ImageItem.Add(newImage);
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(new { count = files.First().FileName, size, filePath });
        }
    }
}
