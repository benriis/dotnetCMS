using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WoWScriptApi.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace WoWScriptApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly ScriptContext _context;
        private readonly PasswordHasher<string> hash = new PasswordHasher<string>();
        private readonly string SIGNING_KEY;

        public AuthController(ScriptContext context)
        {
            _context = context;
            SIGNING_KEY = Environment.GetEnvironmentVariable("SIGNING_KEY");
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] User user)
        {

            PasswordVerificationResult verifyPassword = PasswordVerificationResult.Failed;

            if (user == null)
            {
                return BadRequest("Invalid client request");
            }

            var userFromDb = await _context.Users.SingleOrDefaultAsync(u => u.Username == user.Username);
            
            if (userFromDb != null)
            {
                verifyPassword = hash.VerifyHashedPassword("", userFromDb.Password, user.Password);
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromDb.Id.ToString())
            };

            if (verifyPassword == PasswordVerificationResult.Success)
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SIGNING_KEY));
                var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var tokenOptions = new JwtSecurityToken(
                        issuer: "https://localhost:44348",
                        audience: "https://localhost:44348",
                        claims,
                        expires: DateTime.Now.AddMinutes(60),
                        signingCredentials: signingCredentials
                    );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                return Ok(new { Token = tokenString, Name = user.Username });
            } else
            {
                return Unauthorized();
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser([FromBody] User user)
        {
            bool userAlreadyExists = _context.Users.Any(u => user.Username == u.Username);

            if (userAlreadyExists)
            {
                return Forbid();
            }

            string hashedUSerPassword = hash.HashPassword("", user.Password);
            user.Password = hashedUSerPassword;

            //PasswordVerificationResult b = hash.VerifyHashedPassword("", a, "hey");

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }

        [Authorize]
        [HttpGet("a")]
        public async Task<ActionResult<IEnumerable<ScriptItem>>> GetUsers()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var author = await _context.Users.FindAsync(userId);

            System.Diagnostics.Debug.WriteLine("Value:");
            foreach (var value in User.Identities)
            {
                System.Diagnostics.Debug.WriteLine("Claim: " + value);
            }

            var userName = await _context.ScriptItems.Where(x => x.Author == author).ToListAsync();
            System.Diagnostics.Debug.WriteLine("Username: " + userName.Count());

            userName.Count();



            return await _context.ScriptItems.Where(x => x.Author == author).ToListAsync();

        }

    }
}