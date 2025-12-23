using EhicBackend.Data;
using EhicBackend.Entities;
using EhicBackend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EhicBackend.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly ApplicationDbContext _context;

        public QuestionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QuestionDto>> GetAllQuestionsAsync()
        {
            return await _context.Questions
                .Include(q => q.AnswerChoices)
                .Where(q => q.IsActive)
                .Select(q => MapToDto(q))
                .ToListAsync();
        }

        public async Task<QuestionDto?> GetQuestionByIdAsync(int id)
        {
            var question = await _context.Questions
                .Include(q => q.AnswerChoices)
                .FirstOrDefaultAsync(q => q.Id == id && q.IsActive);

            return question != null ? MapToDto(question) : null;
        }

        public async Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto createQuestionDto, int createdBy)
        {
            var question = new Question
            {
                QuestionText = createQuestionDto.QuestionText,
                QuestionType = createQuestionDto.QuestionType,
                Category = createQuestionDto.Category,
                Difficulty = createQuestionDto.Difficulty,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            // Add answer choices
            foreach (var choiceDto in createQuestionDto.AnswerChoices)
            {
                var answerChoice = new AnswerChoice
                {
                    QuestionId = question.Id,
                    ChoiceText = choiceDto.ChoiceText,
                    IsCorrect = choiceDto.IsCorrect,
                    DisplayOrder = choiceDto.DisplayOrder
                };
                _context.AnswerChoices.Add(answerChoice);
            }

            await _context.SaveChangesAsync();

            return await GetQuestionByIdAsync(question.Id) ?? throw new InvalidOperationException("Failed to create question");
        }

        public async Task<QuestionDto?> UpdateQuestionAsync(int id, UpdateQuestionDto updateQuestionDto)
        {
            var question = await _context.Questions
                .Include(q => q.AnswerChoices)
                .FirstOrDefaultAsync(q => q.Id == id && q.IsActive);

            if (question == null) return null;

            question.QuestionText = updateQuestionDto.QuestionText;
            question.QuestionType = updateQuestionDto.QuestionType;
            question.Category = updateQuestionDto.Category;
            question.Difficulty = updateQuestionDto.Difficulty;
            question.UpdatedAt = DateTime.UtcNow;

            // Update answer choices
            _context.AnswerChoices.RemoveRange(question.AnswerChoices);
            
            foreach (var choiceDto in updateQuestionDto.AnswerChoices)
            {
                var answerChoice = new AnswerChoice
                {
                    QuestionId = question.Id,
                    ChoiceText = choiceDto.ChoiceText,
                    IsCorrect = choiceDto.IsCorrect,
                    DisplayOrder = choiceDto.DisplayOrder
                };
                _context.AnswerChoices.Add(answerChoice);
            }

            await _context.SaveChangesAsync();
            return await GetQuestionByIdAsync(id);
        }

        public async Task<bool> DeleteQuestionAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return false;

            question.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<QuestionDto>> GetQuestionsByCreatorAsync(int creatorId)
        {
            return await _context.Questions
                .Include(q => q.AnswerChoices)
                .Where(q => q.CreatedBy == creatorId && q.IsActive)
                .Select(q => MapToDto(q))
                .ToListAsync();
        }

        public async Task<IEnumerable<QuestionDto>> GetQuestionsByCategoryAsync(string category)
        {
            return await _context.Questions
                .Include(q => q.AnswerChoices)
                .Where(q => q.Category == category && q.IsActive)
                .Select(q => MapToDto(q))
                .ToListAsync();
        }

        private static QuestionDto MapToDto(Question question)
        {
            return new QuestionDto
            {
                Id = question.Id,
                QuestionText = question.QuestionText,
                QuestionType = question.QuestionType,
                Category = question.Category,
                Difficulty = question.Difficulty,
                CreatedBy = question.CreatedBy,
                CreatedAt = question.CreatedAt,
                AnswerChoices = question.AnswerChoices.Select(ac => new AnswerChoiceDto
                {
                    Id = ac.Id,
                    ChoiceText = ac.ChoiceText,
                    IsCorrect = ac.IsCorrect,
                    DisplayOrder = ac.DisplayOrder
                }).ToList()
            };
        }
    }
}