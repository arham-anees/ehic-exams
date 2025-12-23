using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EhicBackend.Services;
using EhicBackend.DTOs;
using System.Security.Claims;

namespace EhicBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BaptismEligibilityController : ControllerBase
    {
        private readonly IBaptismEligibilityService _baptismService;

        public BaptismEligibilityController(IBaptismEligibilityService baptismService)
        {
            _baptismService = baptismService;
        }

        [HttpGet("eligible")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<IEnumerable<BaptismEligibilityDto>>> GetEligibleStudents()
        {
            var eligibleStudents = await _baptismService.GetEligibleStudentsAsync();
            return Ok(eligibleStudents);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<IEnumerable<BaptismEligibilityDto>>> GetAllEligibilityRecords()
        {
            var records = await _baptismService.GetAllEligibilityRecordsAsync();
            return Ok(records);
        }

        [HttpGet("my-eligibility")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<BaptismEligibilityDto>> GetMyEligibility()
        {
            var userId = GetCurrentUserId();
            var eligibility = await _baptismService.GetUserEligibilityAsync(userId);
            
            if (eligibility == null)
            {
                return Ok(new { message = "No eligibility record found. Complete an exam to check your status." });
            }
            
            return Ok(eligibility);
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<BaptismEligibilityDto>> GetUserEligibility(int userId)
        {
            var eligibility = await _baptismService.GetUserEligibilityAsync(userId);
            
            if (eligibility == null)
            {
                return NotFound(new { message = "No eligibility record found for this user" });
            }
            
            return Ok(eligibility);
        }

        [HttpPut("{eligibilityId}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult> UpdateEligibility(int eligibilityId, UpdateEligibilityDto updateDto)
        {
            var result = await _baptismService.UpdateEligibilityAsync(eligibilityId, updateDto.IsEligible, updateDto.Notes);
            
            if (!result)
            {
                return NotFound(new { message = "Eligibility record not found" });
            }
            
            return Ok(new { message = "Eligibility updated successfully" });
        }

        [HttpPost("{eligibilityId}/issue-certificate")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult> IssueCertificate(int eligibilityId, IssueCertificateDto issueDto)
        {
            var result = await _baptismService.IssueCertificateAsync(eligibilityId, issueDto.Notes);
            
            if (!result)
            {
                return BadRequest(new { message = "Cannot issue certificate. Student may not be eligible or record not found." });
            }
            
            return Ok(new { message = "Certificate issued successfully" });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}