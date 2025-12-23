namespace EhicBackend.DTOs
{
    public class ExamDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TotalQuestions { get; set; }
        public int QuestionsPerExam { get; set; }
        public int TimeLimit { get; set; }
        public decimal PassingPercentage { get; set; }
        public bool IsOpen { get; set; }
        public int MaxAttempts { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int QuestionCount { get; set; }
    }

    public class CreateExamDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TotalQuestions { get; set; }
        public int QuestionsPerExam { get; set; }
        public int TimeLimit { get; set; } = 60;
        public decimal PassingPercentage { get; set; } = 80;
        public int MaxAttempts { get; set; } = 1;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UpdateExamDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TotalQuestions { get; set; }
        public int QuestionsPerExam { get; set; }
        public int TimeLimit { get; set; }
        public decimal PassingPercentage { get; set; }
        public int MaxAttempts { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}