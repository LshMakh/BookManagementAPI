using BookManagementAPI.Data;
using BookManagementAPI.Models;
using BookManagementAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BookDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(BookDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest("Username Already Exists!");
            if(await _context.Users.AnyAsync(u=>u.Email == request.Email))
                return BadRequest("Email Already Exists!");

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new {token = _jwtService.GenerateToken(user) });
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || request.Password != user.Password )
                return Unauthorized("Invalid username or password");

            return Ok(new { token = _jwtService.GenerateToken(user) });
        }
    }
}
