using LearnAvaloniaApi.Data;
using LearnAvaloniaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearnAvaloniaApi.Controllers
{
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
            var projects = await _context.Projects.ToListAsync();
            return Ok(projects);
        }

        // Return single project
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiProject>> GetProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        // Create a Project
        [HttpPost]
        public async Task<ActionResult<ApiProject>> CreateProject(ApiProject project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new {id = project.Id }, project);
        }

        // Update a project
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiProject>> UpdateProject(int id,  ApiProject project)
        {
            if (id != project.Id)
            {
                return BadRequest("Id mismatch");
            }

            var dbProject = await _context.Projects.FindAsync(id);
            if (dbProject == null)
            {
                return NotFound();
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
            var project = await _context.Projects.FindAsync(id);

            if (project == null) return NotFound();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
