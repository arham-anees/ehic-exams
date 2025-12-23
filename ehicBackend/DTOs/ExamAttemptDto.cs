namespace EhicBackend.DTOs
{
    public class ExamAttemptDto
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int AttemptNumber { get; set; }
        public int Score { get; set; }
        public decimal Percentage { get; set; }
        public bool Passed { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? DueAt { get; set; }
    }

    public class StudentExamDto
    {
        public int AttemptId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int TimeLimit { get; set; }
        public DateTime? DueAt { get; set; }
        public List<StudentQuestionDto> Questions { get; set; } = new();
    }

    public class StudentQuestionDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int QuestionOrder { get; set; }
        public List<StudentAnswerChoiceDto> AnswerChoices { get; set; } = new();
        public int? SelectedAnswerId { get; set; }
    }

    public class StudentAnswerChoiceDto
    {
        public int Id { get; set; }
        public string ChoiceText { get; set; } = string.Empty;
    }

    public class SubmitAnswerDto
    {
        public int QuestionId { get; set; }
        public int SelectedAnswerId { get; set; }
    }

    public class RankingDto
    {
        public int Rank { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Score { get; set; }
        public decimal Percentage { get; set; }
        public bool Passed { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}