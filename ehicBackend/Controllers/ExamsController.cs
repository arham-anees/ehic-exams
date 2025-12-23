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
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamsController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<IEnumerable<ExamDto>>> GetExams()
        {
            var exams = await _examService.GetAllExamsAsync();
            return Ok(exams);
        }

        [HttpGet("available")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<ExamDto>>> GetAvailableExams()
        {
            var exams = await _examService.GetActiveExamsForStudentAsync();
            return Ok(exams);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExamDto>> GetExam(int id)
        {
            var exam = await _examService.GetExamByIdAsync(id);
            if (exam == null)
            {
                return NotFound();
            }
            return Ok(exam);
        }

        [HttpGet("by-creator/{creatorId}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<IEnumerable<ExamDto>>> GetExamsByCreator(int creatorId)
        {
            var exams = await _examService.GetExamsByCreatorAsync(creatorId);
            return Ok(exams);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<ExamDto>> CreateExam(CreateExamDto createExamDto)
        {
            var userId = GetCurrentUserId();
            var exam = await _examService.CreateExamAsync(createExamDto, userId);
            return CreatedAtAction(nameof(GetExam), new { id = exam.Id }, exam);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<ExamDto>> UpdateExam(int id, UpdateExamDto updateExamDto)
        {
            var exam = await _examService.UpdateExamAsync(id, updateExamDto);
            if (exam == null)
            {
                return NotFound();
            }
            return Ok(exam);
        }

        [HttpPatch("{id}/toggle-status")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult> ToggleExamStatus(int id, [FromBody] bool isOpen)
        {
            var result = await _examService.ToggleExamStatusAsync(id, isOpen);
            if (!result)
            {
                return NotFound();
            }
            return Ok(new { message = $"Exam {(isOpen ? "opened" : "closed")} successfully" });
        }

        [HttpPost("{id}/questions")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult> AddQuestionsToExam(int id, [FromBody] int[] questionIds)
        {
            var result = await _examService.AddQuestionsToExamAsync(id, questionIds);
            if (!result)
            {
                return NotFound();
            }
            return Ok(new { message = "Questions added successfully" });
        }

        [HttpDelete("{id}/questions")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult> RemoveQuestionsFromExam(int id, [FromBody] int[] questionIds)
        {
            var result = await _examService.RemoveQuestionsFromExamAsync(id, questionIds);
            if (!result)
            {
                return NotFound();
            }
            return Ok(new { message = "Questions removed successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult> DeleteExam(int id)
        {
            var result = await _examService.DeleteExamAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}