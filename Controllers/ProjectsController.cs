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
    public class ProjectsController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public ProjectsController(ApiDbContext context) { _context = context; }

        // Return all projects
        [HttpGet]
        public async Task<ActionResult<List<ApiProject>>> GetAllProjects()
        {
            int? userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized("Invalid User token");
            }

            var projects = await _context.Projects
                .Where(p => p.UserId == userId)
                .ToListAsync();

            return Ok(projects);
        }

        // Return single project
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiProject>> GetProject(int id)
        {
            int? userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }
            
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            if (project.UserId != userId)
            {
                return Unauthorized("Not authorized");
            }

            return Ok(project);
        }

        // Create a Project
        [HttpPost]
        public async Task<ActionResult<ApiProject>> CreateProject(ApiProject project)
        {
            int? userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            // Set userId for the project
            project.UserId = (int)userId;

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new {id = project.Id }, project);
        }

        // Update a project
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiProject>> UpdateProject(int id,  ApiProject project)
        {
            int? userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            if (id != project.Id)
            {
                return BadRequest("Id mismatch");
            }

            var dbProject = await _context.Projects.FindAsync(id);
            if (dbProject == null)
            {
                return NotFound();
            }

            if (dbProject.UserId != userId)
            {
                return Unauthorized("Unauthorized access");
            }

            dbProject.Name = project.Name;
            dbProject.Description = project.Description;
            dbProject.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult>DeleteProject(int id)
        {
            int? userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }
            var project = await _context.Projects.FindAsync(id);

            if (project?.UserId != userId)
            {
                return Unauthorized("Not Authorized");
            }

            if (project == null) return NotFound();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public int? GetCurrentUserId()
        {
            // Gets current userId Claim
            var userIdClaim = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?
                .Value;

            // Returns userid from claim or null
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }

    }
}
