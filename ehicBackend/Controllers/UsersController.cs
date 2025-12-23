using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EhicBackend.Data;
using EhicBackend.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EhicBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<IEnumerable<object>>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.IsActive)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.IsActive,
                    Role = u.Role.Name
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("students")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<IEnumerable<object>>> GetStudents()
        {
            var students = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.IsActive && u.Role.Name == "Student")
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    ExamsTaken = _context.ExamAttempts.Count(ea => ea.UserId == u.Id && ea.IsCompleted),
                    ExamsPassed = _context.ExamAttempts.Count(ea => ea.UserId == u.Id && ea.IsCompleted && ea.Passed),
                    AverageScore = _context.ExamAttempts
                        .Where(ea => ea.UserId == u.Id && ea.IsCompleted)
                        .Average(ea => (double?)ea.Percentage) ?? 0,
                    IsEligibleForBaptism = _context.BaptismEligibilities
                        .Any(be => be.UserId == u.Id && be.IsEligible && be.IsActive)
                })
                .ToListAsync();

            return Ok(students);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<object>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Id == id && u.IsActive)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.IsActive,
                    Role = u.Role.Name
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("profile")]
        public async Task<ActionResult<object>> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            var user = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    Role = u.Role.Name
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut("{id}/toggle-status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ToggleUserStatus(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"User {(user.IsActive ? "activated" : "deactivated")} successfully" });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}