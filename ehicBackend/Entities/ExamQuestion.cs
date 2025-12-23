using EhicBackend.Entities.Common;

namespace EhicBackend.Entities
{
    public class ExamQuestion : BaseEntity
    {
        public int ExamId { get; set; }
        public int QuestionId { get; set; }

        // Navigation properties
        public virtual Exam Exam { get; set; } = null!;
        public virtual Question Question { get; set; } = null!;
    }
}