using EhicBackend.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace EhicBackend.Entities
{
    public class ExamAttempt : AuditBaseEntity
    {
        public int UserId { get; set; }
        public int ExamId { get; set; }
        public int AttemptNumber { get; set; }
        public int Score { get; set; } = 0;
        public decimal Percentage { get; set; } = 0;
        public bool Passed { get; set; } = false;
        public bool IsCompleted { get; set; } = false;

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public DateTime? DueAt { get; set; } // StartedAt + TimeLimit

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Exam Exam { get; set; } = null!;
        public virtual ICollection<ExamResponse> Responses { get; set; } = new List<ExamResponse>();
    }
}