namespace EhicBackend.DTOs
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Difficulty { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<AnswerChoiceDto> AnswerChoices { get; set; } = new();
    }

    public class CreateQuestionDto
    {
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = "MultipleChoice";
        public string? Category { get; set; }
        public string? Difficulty { get; set; }
        public List<CreateAnswerChoiceDto> AnswerChoices { get; set; } = new();
    }

    public class UpdateQuestionDto
    {
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Difficulty { get; set; }
        public List<CreateAnswerChoiceDto> AnswerChoices { get; set; } = new();
    }

    public class AnswerChoiceDto
    {
        public int Id { get; set; }
        public string ChoiceText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class CreateAnswerChoiceDto
    {
        public string ChoiceText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int DisplayOrder { get; set; }
    }
}