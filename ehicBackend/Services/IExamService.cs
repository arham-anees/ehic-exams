using EhicBackend.DTOs;

namespace EhicBackend.Services
{
    public interface IExamService
    {
        Task<IEnumerable<ExamDto>> GetAllExamsAsync();
        Task<ExamDto?> GetExamByIdAsync(int id);
        Task<ExamDto> CreateExamAsync(CreateExamDto createExamDto, int createdBy);
        Task<ExamDto?> UpdateExamAsync(int id, UpdateExamDto updateExamDto);
        Task<bool> DeleteExamAsync(int id);
        Task<bool> ToggleExamStatusAsync(int id, bool isOpen);
        Task<IEnumerable<ExamDto>> GetExamsByCreatorAsync(int creatorId);
        Task<IEnumerable<ExamDto>> GetActiveExamsForStudentAsync();
        Task<bool> AddQuestionsToExamAsync(int examId, IEnumerable<int> questionIds);
        Task<bool> RemoveQuestionsFromExamAsync(int examId, IEnumerable<int> questionIds);
    }
}