using EhicBackend.Data;
using EhicBackend.Entities;
using EhicBackend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EhicBackend.Services
{
    public class ExamAttemptService : IExamAttemptService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBaptismEligibilityService _baptismService;

        public ExamAttemptService(ApplicationDbContext context, IBaptismEligibilityService baptismService)
        {
            _context = context;
            _baptismService = baptismService;
        }

        public async Task<ExamAttemptDto?> StartExamAsync(int examId, int userId)
        {
            var exam = await _context.Exams
                .Include(e => e.ExamQuestions)
                .ThenInclude(eq => eq.Question)
                .ThenInclude(q => q.AnswerChoices)
                .FirstOrDefaultAsync(e => e.Id == examId && e.IsActive && e.IsOpen);

            if (exam == null) return null;

            // Check if user has exceeded max attempts
            var attemptCount = await _context.ExamAttempts
                .CountAsync(ea => ea.ExamId == examId && ea.UserId == userId);

            if (attemptCount >= exam.MaxAttempts) return null;

            // Check if there's an active attempt
            var activeAttempt = await _context.ExamAttempts
                .FirstOrDefaultAsync(ea => ea.ExamId == examId && ea.UserId == userId && !ea.IsCompleted);

            if (activeAttempt != null) return MapToDto(activeAttempt);

            // Create new attempt
            var newAttempt = new ExamAttempt
            {
                ExamId = examId,
                UserId = userId,
                AttemptNumber = attemptCount + 1,
                StartedAt = DateTime.UtcNow,
                DueAt = DateTime.UtcNow.AddMinutes(exam.TimeLimit),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.ExamAttempts.Add(newAttempt);
            await _context.SaveChangesAsync();

            // Generate random questions for this attempt
            var randomQuestions = exam.ExamQuestions
                .OrderBy(x => Guid.NewGuid())
                .Take(exam.QuestionsPerExam)
                .ToList();

            // Create exam responses with randomized answer order
            for (int i = 0; i < randomQuestions.Count; i++)
            {
                var examQuestion = randomQuestions[i];
                var response = new ExamResponse
                {
                    ExamAttemptId = newAttempt.Id,
                    QuestionId = examQuestion.QuestionId,
                    QuestionOrder = i + 1,
                    SelectedAnswerId = 0 // Will be updated when user answers
                };
                _context.ExamResponses.Add(response);
            }

            await _context.SaveChangesAsync();
            return await GetExamAttemptWithDetailsAsync(newAttempt.Id);
        }

        public async Task<ExamAttemptDto?> GetActiveExamAttemptAsync(int examId, int userId)
        {
            var attempt = await _context.ExamAttempts
                .FirstOrDefaultAsync(ea => ea.ExamId == examId && ea.UserId == userId && !ea.IsCompleted);

            return attempt != null ? await GetExamAttemptWithDetailsAsync(attempt.Id) : null;
        }

        public async Task<bool> SubmitAnswerAsync(int attemptId, int questionId, int selectedAnswerId)
        {
            var response = await _context.ExamResponses
                .Include(er => er.SelectedAnswer)
                .FirstOrDefaultAsync(er => er.ExamAttemptId == attemptId && er.QuestionId == questionId);

            if (response == null) return false;

            var answerChoice = await _context.AnswerChoices.FindAsync(selectedAnswerId);
            if (answerChoice == null) return false;

            response.SelectedAnswerId = selectedAnswerId;
            response.IsCorrect = answerChoice.IsCorrect;
            response.AnsweredAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ExamAttemptDto?> CompleteExamAsync(int attemptId)
        {
            var attempt = await _context.ExamAttempts
                .Include(ea => ea.Responses)
                .Include(ea => ea.Exam)
                .FirstOrDefaultAsync(ea => ea.Id == attemptId);

            if (attempt == null || attempt.IsCompleted) return null;

            // Calculate score
            var totalQuestions = attempt.Responses.Count;
            var correctAnswers = attempt.Responses.Count(r => r.IsCorrect);
            var percentage = totalQuestions > 0 ? (decimal)correctAnswers / totalQuestions * 100 : 0;

            attempt.Score = correctAnswers;
            attempt.Percentage = percentage;
            attempt.Passed = percentage >= attempt.Exam.PassingPercentage;
            attempt.IsCompleted = true;
            attempt.CompletedAt = DateTime.UtcNow;
            attempt.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Create baptism eligibility record
            await _baptismService.DetermineBaptismEligibilityAsync(attempt.Id);

            return await GetExamAttemptWithDetailsAsync(attemptId);
        }

        public async Task<IEnumerable<ExamAttemptDto>> GetUserExamAttemptsAsync(int userId)
        {
            var attempts = await _context.ExamAttempts
                .Include(ea => ea.Exam)
                .Where(ea => ea.UserId == userId)
                .OrderByDescending(ea => ea.StartedAt)
                .ToListAsync();

            return attempts.Select(MapToDto);
        }

        public async Task<IEnumerable<ExamAttemptDto>> GetExamAttemptsForExamAsync(int examId)
        {
            var attempts = await _context.ExamAttempts
                .Include(ea => ea.User)
                .Include(ea => ea.Exam)
                .Where(ea => ea.ExamId == examId && ea.IsCompleted)
                .OrderByDescending(ea => ea.Percentage)
                .ToListAsync();

            return attempts.Select(MapToDto);
        }

        public async Task<IEnumerable<RankingDto>> GetExamRankingsAsync(int examId)
        {
            var attempts = await _context.ExamAttempts
                .Include(ea => ea.User)
                .Where(ea => ea.ExamId == examId && ea.IsCompleted)
                .OrderByDescending(ea => ea.Percentage)
                .ThenByDescending(ea => ea.Score)
                .ToListAsync();

            var rankings = new List<RankingDto>();
            for (int i = 0; i < attempts.Count; i++)
            {
                var attempt = attempts[i];
                rankings.Add(new RankingDto
                {
                    Rank = i + 1,
                    UserId = attempt.UserId,
                    UserName = $"{attempt.User.FirstName} {attempt.User.LastName}",
                    Score = attempt.Score,
                    Percentage = attempt.Percentage,
                    Passed = attempt.Passed,
                    CompletedAt = attempt.CompletedAt ?? DateTime.MinValue
                });
            }

            return rankings;
        }

        public async Task<bool> ResetExamAttemptAsync(int userId, int examId)
        {
            var attempts = await _context.ExamAttempts
                .Where(ea => ea.UserId == userId && ea.ExamId == examId)
                .ToListAsync();

            _context.ExamAttempts.RemoveRange(attempts);

            // Also remove baptism eligibility records
            var eligibilities = await _context.BaptismEligibilities
                .Where(be => be.UserId == userId && attempts.Select(a => a.Id).Contains(be.ExamAttemptId))
                .ToListAsync();

            _context.BaptismEligibilities.RemoveRange(eligibilities);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<StudentExamDto?> GetStudentExamQuestionsAsync(int attemptId)
        {
            var attempt = await _context.ExamAttempts
                .Include(ea => ea.Exam)
                .Include(ea => ea.Responses)
                .ThenInclude(er => er.Question)
                .ThenInclude(q => q.AnswerChoices)
                .FirstOrDefaultAsync(ea => ea.Id == attemptId);

            if (attempt == null || attempt.IsCompleted) return null;

            var questions = attempt.Responses
                .OrderBy(r => r.QuestionOrder)
                .Select(r => new StudentQuestionDto
                {
                    Id = r.Question.Id,
                    QuestionText = r.Question.QuestionText,
                    QuestionOrder = r.QuestionOrder,
                    AnswerChoices = r.Question.AnswerChoices
                        .OrderBy(x => Guid.NewGuid()) // Randomize answer order
                        .Select(ac => new StudentAnswerChoiceDto
                        {
                            Id = ac.Id,
                            ChoiceText = ac.ChoiceText
                        }).ToList(),
                    SelectedAnswerId = r.SelectedAnswerId > 0 ? r.SelectedAnswerId : null
                }).ToList();

            return new StudentExamDto
            {
                AttemptId = attempt.Id,
                ExamTitle = attempt.Exam.Title,
                TotalQuestions = questions.Count,
                TimeLimit = attempt.Exam.TimeLimit,
                DueAt = attempt.DueAt,
                Questions = questions
            };
        }

        private async Task<ExamAttemptDto?> GetExamAttemptWithDetailsAsync(int attemptId)
        {
            var attempt = await _context.ExamAttempts
                .Include(ea => ea.Exam)
                .Include(ea => ea.User)
                .FirstOrDefaultAsync(ea => ea.Id == attemptId);

            return attempt != null ? MapToDto(attempt) : null;
        }

        private static ExamAttemptDto MapToDto(ExamAttempt attempt)
        {
            return new ExamAttemptDto
            {
                Id = attempt.Id,
                ExamId = attempt.ExamId,
                ExamTitle = attempt.Exam?.Title ?? string.Empty,
                UserId = attempt.UserId,
                UserName = attempt.User != null ? $"{attempt.User.FirstName} {attempt.User.LastName}" : string.Empty,
                AttemptNumber = attempt.AttemptNumber,
                Score = attempt.Score,
                Percentage = attempt.Percentage,
                Passed = attempt.Passed,
                IsCompleted = attempt.IsCompleted,
                StartedAt = attempt.StartedAt,
                CompletedAt = attempt.CompletedAt,
                DueAt = attempt.DueAt
            };
        }
    }
}