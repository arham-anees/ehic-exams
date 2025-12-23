using EhicBackend.DTOs;

namespace EhicBackend.Services
{
    public interface IExamAttemptService
    {
        Task<ExamAttemptDto?> StartExamAsync(int examId, int userId);
        Task<ExamAttemptDto?> GetActiveExamAttemptAsync(int examId, int userId);
        Task<bool> SubmitAnswerAsync(int attemptId, int questionId, int selectedAnswerId);
        Task<ExamAttemptDto?> CompleteExamAsync(int attemptId);
        Task<IEnumerable<ExamAttemptDto>> GetUserExamAttemptsAsync(int userId);
        Task<IEnumerable<ExamAttemptDto>> GetExamAttemptsForExamAsync(int examId);
        Task<IEnumerable<RankingDto>> GetExamRankingsAsync(int examId);
        Task<bool> ResetExamAttemptAsync(int userId, int examId);
        Task<StudentExamDto?> GetStudentExamQuestionsAsync(int attemptId);
    }
}