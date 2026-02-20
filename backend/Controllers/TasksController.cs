using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManager.Models;
using TaskManager.Data;

namespace TaskManager.API
{
    [Route("api/tasks")]
    [ApiController]
    public class TasksController(ApplicationDbContext context) : ControllerBase{
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            
            var tasks = await context.Tasks.ToListAsync();
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskItem task)
        {
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, task);
        }

        [HttpPut("{id}")] 
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskItem updated)
        {
            var task = await context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            task.Title = updated.Title;
            task.IsDone = updated.IsDone;
            await context.SaveChangesAsync();

            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            context.Tasks.Remove(task);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }

}
