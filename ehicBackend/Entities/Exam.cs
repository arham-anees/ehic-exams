using EhicBackend.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace EhicBackend.Entities
{
    public class Exam : AuditBaseEntity
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public int TotalQuestions { get; set; } // Total questions in pool
        public int QuestionsPerExam { get; set; } // Questions per student exam
        public int TimeLimit { get; set; } = 60; // In minutes
        public decimal PassingPercentage { get; set; } = 80;
        public bool IsOpen { get; set; } = false; // Can students take it now?
        public int MaxAttempts { get; set; } = 1;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Navigation properties
        public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
    }
}