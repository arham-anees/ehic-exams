using EhicBackend.Data;
using EhicBackend.Entities;
using EhicBackend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EhicBackend.Services
{
    public class ExamService : IExamService
    {
        private readonly ApplicationDbContext _context;

        public ExamService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExamDto>> GetAllExamsAsync()
        {
            return await _context.Exams
                .Include(e => e.ExamQuestions)
                .Where(e => e.IsActive)
                .Select(e => MapToDto(e))
                .ToListAsync();
        }

        public async Task<ExamDto?> GetExamByIdAsync(int id)
        {
            var exam = await _context.Exams
                .Include(e => e.ExamQuestions)
                .ThenInclude(eq => eq.Question)
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);

            return exam != null ? MapToDto(exam) : null;
        }

        public async Task<ExamDto> CreateExamAsync(CreateExamDto createExamDto, int createdBy)
        {
            var exam = new Exam
            {
                Title = createExamDto.Title,
                Description = createExamDto.Description,
                TotalQuestions = createExamDto.TotalQuestions,
                QuestionsPerExam = createExamDto.QuestionsPerExam,
                TimeLimit = createExamDto.TimeLimit,
                PassingPercentage = createExamDto.PassingPercentage,
                MaxAttempts = createExamDto.MaxAttempts,
                StartDate = createExamDto.StartDate,
                EndDate = createExamDto.EndDate,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();

            return await GetExamByIdAsync(exam.Id) ?? throw new InvalidOperationException("Failed to create exam");
        }

        public async Task<ExamDto?> UpdateExamAsync(int id, UpdateExamDto updateExamDto)
        {
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
            if (exam == null) return null;

            exam.Title = updateExamDto.Title;
            exam.Description = updateExamDto.Description;
            exam.TotalQuestions = updateExamDto.TotalQuestions;
            exam.QuestionsPerExam = updateExamDto.QuestionsPerExam;
            exam.TimeLimit = updateExamDto.TimeLimit;
            exam.PassingPercentage = updateExamDto.PassingPercentage;
            exam.MaxAttempts = updateExamDto.MaxAttempts;
            exam.StartDate = updateExamDto.StartDate;
            exam.EndDate = updateExamDto.EndDate;
            exam.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetExamByIdAsync(id);
        }

        public async Task<bool> DeleteExamAsync(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null) return false;

            exam.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleExamStatusAsync(int id, bool isOpen)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null) return false;

            exam.IsOpen = isOpen;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ExamDto>> GetExamsByCreatorAsync(int creatorId)
        {
            return await _context.Exams
                .Include(e => e.ExamQuestions)
                .Where(e => e.CreatedBy == creatorId && e.IsActive)
                .Select(e => MapToDto(e))
                .ToListAsync();
        }

        public async Task<IEnumerable<ExamDto>> GetActiveExamsForStudentAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Exams
                .Where(e => e.IsActive && e.IsOpen && 
                           (e.StartDate == null || e.StartDate <= now) &&
                           (e.EndDate == null || e.EndDate >= now))
                .Select(e => MapToDto(e))
                .ToListAsync();
        }

        public async Task<bool> AddQuestionsToExamAsync(int examId, IEnumerable<int> questionIds)
        {
            var exam = await _context.Exams.FindAsync(examId);
            if (exam == null) return false;

            foreach (var questionId in questionIds)
            {
                var existingExamQuestion = await _context.ExamQuestions
                    .FirstOrDefaultAsync(eq => eq.ExamId == examId && eq.QuestionId == questionId);

                if (existingExamQuestion == null)
                {
                    _context.ExamQuestions.Add(new ExamQuestion
                    {
                        ExamId = examId,
                        QuestionId = questionId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveQuestionsFromExamAsync(int examId, IEnumerable<int> questionIds)
        {
            var examQuestions = await _context.ExamQuestions
                .Where(eq => eq.ExamId == examId && questionIds.Contains(eq.QuestionId))
                .ToListAsync();

            _context.ExamQuestions.RemoveRange(examQuestions);
            await _context.SaveChangesAsync();
            return true;
        }

        private static ExamDto MapToDto(Exam exam)
        {
            return new ExamDto
            {
                Id = exam.Id,
                Title = exam.Title,
                Description = exam.Description,
                TotalQuestions = exam.TotalQuestions,
                QuestionsPerExam = exam.QuestionsPerExam,
                TimeLimit = exam.TimeLimit,
                PassingPercentage = exam.PassingPercentage,
                IsOpen = exam.IsOpen,
                MaxAttempts = exam.MaxAttempts,
                CreatedBy = exam.CreatedBy,
                CreatedAt = exam.CreatedAt,
                StartDate = exam.StartDate,
                EndDate = exam.EndDate,
                QuestionCount = exam.ExamQuestions.Count
            };
        }
    }
}