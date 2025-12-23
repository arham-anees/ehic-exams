using EhicBackend.DTOs;

namespace EhicBackend.Services
{
    public interface IQuestionService
    {
        Task<IEnumerable<QuestionDto>> GetAllQuestionsAsync();
        Task<QuestionDto?> GetQuestionByIdAsync(int id);
        Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto createQuestionDto, int createdBy);
        Task<QuestionDto?> UpdateQuestionAsync(int id, UpdateQuestionDto updateQuestionDto);
        Task<bool> DeleteQuestionAsync(int id);
        Task<IEnumerable<QuestionDto>> GetQuestionsByCreatorAsync(int creatorId);
        Task<IEnumerable<QuestionDto>> GetQuestionsByCategoryAsync(string category);
    }
}