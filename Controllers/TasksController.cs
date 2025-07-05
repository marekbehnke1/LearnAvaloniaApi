using System.Runtime.InteropServices;
using System.Security.Claims;
using LearnAvaloniaApi.Data;
using LearnAvaloniaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearnAvaloniaApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ApiDbContext _context;


        public TasksController(ApiDbContext context)
        {
            _context = context;
        }

        // Return all tasks
        // TODO: Modify to return tasks only for the current user
        [HttpGet]
        public async Task<ActionResult<List<ApiTask>>> GetAllTasks()
        {
            // Check for valid token and get Id
            int? userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            var tasks = await _context.Tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();
            
            return Ok(tasks);
        }

        // Return single task by Id.
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiTask>> GetTask(int id)
        {
            // Check for valid token and get Id
            int? userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            if (userId != task.UserId)
            {
                return Unauthorized("Not authorized");
            }
            

            return Ok(task);
        }

        // Create a new task
        [HttpPost]
        public async Task<ActionResult<ApiTask>> CreateTask(ApiTask task)
        {
            // Check for valid token and get Id
            int? userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            task.UserId = (int)userId;

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        // TODO - PUT update task
        [HttpPut("{id}")]
        public async Task <ActionResult<ApiTask>> UpdateTask(int id, ApiTask task)
        {
            // Check for valid token and get Id
            int? userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            // Check if token is owner of task
            if (task.UserId != userId)
            {
                return Unauthorized("Unauthorized access");
            }

            // check task ids match
            if (id != task.Id)
            {
                return BadRequest("Id mismatch!");
            }

            //find task in db
            var dbTask = await _context.Tasks.FindAsync(task.Id);
            if (dbTask == null)
            {
                return NotFound();
            }

            dbTask.Title = task.Title; 
            dbTask.Description = task.Description;
            dbTask.Priority = task.Priority;
            dbTask.DueDate = task.DueDate;
            dbTask.IsCollapsed = task.IsCollapsed;
            dbTask.ProjectId = task.ProjectId;
            dbTask.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            // No content to return - update succesfull
            return NoContent();
        }

        // TODO -  DELETE task
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            int? userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            var task = await _context.Tasks.FindAsync(id);              
            
            if (task?.UserId != userId)
            {
                return Unauthorized("Unauthorized access");
            }

            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            // This returns a no content return - appropriate when deleting
            return NoContent();
        }
        // Helper method to retrieve userId
        private int? GetCurrentUserId()
        {
            var userIdClaim = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?
                .Value;

            return int.TryParse (userIdClaim, out int userId) ? userId : null;
        }


        /// ////////----------TEST METHODS ------------////////////////////////
        [AllowAnonymous]
        [HttpGet("test")]  // ← GET api/tasks/test
        public async Task<ActionResult<ApiTask>> CreateTestTask()
        {
            var testTask = new ApiTask
            {
                Title = "Test Task",
                Description = "Created via API",
                Priority = 1,
                UserId = 1,  // We'll fix user relationships later
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(testTask);
            await _context.SaveChangesAsync();

            return Ok(testTask);
        }

        [AllowAnonymous]
        [HttpGet("create-test-user")]
        public async Task<ActionResult<User>> CreateTestUser()
        {
            var testUser = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                PasswordHash = "temporary",  // We'll do proper hashing later
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(testUser);
            await _context.SaveChangesAsync();

            return Ok(testUser);
        }

        [AllowAnonymous]
        [HttpGet("get-users")]
        public async Task <ActionResult<List<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            return users;
        }

        // debugging endpoint
        [AllowAnonymous]
        [HttpGet("debug")]
        public async Task<ActionResult> DebugTasks()
        {
            var taskCount = await _context.Tasks.CountAsync();
            var userCount = await _context.Users.CountAsync();
            var projectCount = await _context.Projects.CountAsync();

            return Ok(new
            {
                TaskCount = taskCount,
                UserCount = userCount,
                ProjectCount = projectCount
            });
        }
    }

}
