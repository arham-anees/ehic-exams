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
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestions()
        {
            var questions = await _questionService.GetAllQuestionsAsync();
            return Ok(questions);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<QuestionDto>> GetQuestion(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            return Ok(question);
        }

        [HttpGet("by-creator/{creatorId}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestionsByCreator(int creatorId)
        {
            var questions = await _questionService.GetQuestionsByCreatorAsync(creatorId);
            return Ok(questions);
        }

        [HttpGet("by-category/{category}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestionsByCategory(string category)
        {
            var questions = await _questionService.GetQuestionsByCategoryAsync(category);
            return Ok(questions);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<QuestionDto>> CreateQuestion(CreateQuestionDto createQuestionDto)
        {
            var userId = GetCurrentUserId();
            var question = await _questionService.CreateQuestionAsync(createQuestionDto, userId);
            return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<QuestionDto>> UpdateQuestion(int id, UpdateQuestionDto updateQuestionDto)
        {
            var question = await _questionService.UpdateQuestionAsync(id, updateQuestionDto);
            if (question == null)
            {
                return NotFound();
            }
            return Ok(question);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult> DeleteQuestion(int id)
        {
            var result = await _questionService.DeleteQuestionAsync(id);
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