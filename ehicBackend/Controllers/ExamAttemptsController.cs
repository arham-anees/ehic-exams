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
    public class ExamAttemptsController : ControllerBase
    {
        private readonly IExamAttemptService _examAttemptService;

        public ExamAttemptsController(IExamAttemptService examAttemptService)
        {
            _examAttemptService = examAttemptService;
        }

        [HttpPost("start/{examId}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ExamAttemptDto>> StartExam(int examId)
        {
            var userId = GetCurrentUserId();
            var attempt = await _examAttemptService.StartExamAsync(examId, userId);
            
            if (attempt == null)
            {
                return BadRequest(new { message = "Cannot start exam. Check if exam is available and you haven't exceeded max attempts." });
            }
            
            return Ok(attempt);
        }

        [HttpGet("current/{examId}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ExamAttemptDto>> GetCurrentAttempt(int examId)
        {
            var userId = GetCurrentUserId();
            var attempt = await _examAttemptService.GetActiveExamAttemptAsync(examId, userId);
            
            if (attempt == null)
            {
                return NotFound(new { message = "No active attempt found for this exam" });
            }
            
            return Ok(attempt);
        }

        [HttpGet("{attemptId}/questions")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<StudentExamDto>> GetExamQuestions(int attemptId)
        {
            var exam = await _examAttemptService.GetStudentExamQuestionsAsync(attemptId);
            
            if (exam == null)
            {
                return NotFound(new { message = "Exam attempt not found or already completed" });
            }
            
            return Ok(exam);
        }

        [HttpPost("{attemptId}/answer")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> SubmitAnswer(int attemptId, SubmitAnswerDto submitAnswerDto)
        {
            var result = await _examAttemptService.SubmitAnswerAsync(attemptId, submitAnswerDto.QuestionId, submitAnswerDto.SelectedAnswerId);
            
            if (!result)
            {
                return BadRequest(new { message = "Failed to submit answer" });
            }
            
            return Ok(new { message = "Answer submitted successfully" });
        }

        [HttpPost("{attemptId}/complete")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ExamAttemptDto>> CompleteExam(int attemptId)
        {
            var result = await _examAttemptService.CompleteExamAsync(attemptId);
            
            if (result == null)
            {
                return BadRequest(new { message = "Failed to complete exam" });
            }
            
            return Ok(result);
        }

        [HttpGet("my-attempts")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<ExamAttemptDto>>> GetMyAttempts()
        {
            var userId = GetCurrentUserId();
            var attempts = await _examAttemptService.GetUserExamAttemptsAsync(userId);
            return Ok(attempts);
        }

        [HttpGet("exam/{examId}/attempts")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<IEnumerable<ExamAttemptDto>>> GetExamAttempts(int examId)
        {
            var attempts = await _examAttemptService.GetExamAttemptsForExamAsync(examId);
            return Ok(attempts);
        }

        [HttpGet("exam/{examId}/rankings")]
        public async Task<ActionResult<IEnumerable<RankingDto>>> GetExamRankings(int examId)
        {
            var rankings = await _examAttemptService.GetExamRankingsAsync(examId);
            return Ok(rankings);
        }

        [HttpDelete("reset/{userId}/{examId}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult> ResetExamAttempt(int userId, int examId)
        {
            var result = await _examAttemptService.ResetExamAttemptAsync(userId, examId);
            
            if (!result)
            {
                return NotFound(new { message = "No attempts found to reset" });
            }
            
            return Ok(new { message = "Exam attempt reset successfully" });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}