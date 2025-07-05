using LearnAvaloniaApi.Data;
using LearnAvaloniaApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnAvaloniaApi.Models;
using LearnAvaloniaApi.Models.Dtos;
using Microsoft.AspNetCore.Identity.Data;
using System.Threading.Tasks.Dataflow;

namespace LearnAvaloniaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;

        public AuthController(ApiDbContext context, IPasswordService passwordService, IJwtService jwtService)
        {
            _context = context;
            _passwordService = passwordService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(ApiRegisterRequest request)
        {
            // check if the email is already registered
            if (await _context.Users.AnyAsync(User => User.Email == request.Email))
            {
                return BadRequest(new { error = "Email already registerd. Try logging in" });
            }

            // create new user
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = _passwordService.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
                EmailConfirmed = false
            };

            // add user to database
            _context.Users.Add(user);
            // Save db changes - the user object automatically gets updated with an Id at this point
            await _context.SaveChangesAsync();

            // generate token for immediate login
            var token = _jwtService.GenerateToken(user);

            // Return the authresponse containing the token and all the user info
            return Ok(new AuthResponse
            {
                Token = token,
                Message = "Succesfull Registration",
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                }
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(ApiLoginRequest request)
        {
            // Get user from database
            // Returns first user that matches email = request.email
            var user = await _context.Users.FirstOrDefaultAsync(User => User.Email == request.Email);
            
            // checks
            if (user == null || _passwordService.VerifyPassword(request.Password, user.PasswordHash) == false)
            {
                return BadRequest(new { error = "Incorrect Email or Password" });
            }

            // Check if user is still active
            if (!user.IsActive)
            {
                return BadRequest(new { error = "Account Deactivated - Please contact support " });
            }

            // Update the user login
            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // token gen
            var token = _jwtService.GenerateToken(user);
            
            // return auth response
            return Ok(new AuthResponse
            {
                Token = token,
                Message = "Login Succesfull",
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                }
            });    
        }
    }
}
