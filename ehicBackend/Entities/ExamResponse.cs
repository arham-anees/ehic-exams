using EhicBackend.Entities.Common;

namespace EhicBackend.Entities
{
    public class ExamResponse : BaseEntity
    {
        public int ExamAttemptId { get; set; }
        public int QuestionId { get; set; }
        public int SelectedAnswerId { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionOrder { get; set; } // Order in which question appeared
        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ExamAttempt ExamAttempt { get; set; } = null!;
        public virtual Question Question { get; set; } = null!;
        public virtual AnswerChoice SelectedAnswer { get; set; } = null!;
    }
}