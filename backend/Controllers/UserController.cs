using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManager.Models;
using TaskManager.Data;

namespace TaskManager.API
{
    [ApiController]
    [Route("auth")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }

        // Example of adding a new route
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveUsers()
        {
            var activeUsers = await _context.Users
                .Where(u => u.IsActive)
                .ToListAsync();
            return Ok(activeUsers);
        }
    }
}