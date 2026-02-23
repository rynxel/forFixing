using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManager.Models;
using TaskManager.Data;

namespace TaskManager.API
{
    [ApiController]
    [Route("api/users")]
    public class UsersController(ApplicationDbContext context) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetUsers(){
            var users = await context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user){
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }
        [HttpGet("active")] //api/users/active
        public async Task<IActionResult> GetActiveUsers(){
            var activeUsers = await context.Users.Where(u => u.IsActive).ToListAsync();
            return Ok(activeUsers);
        }
    }
}