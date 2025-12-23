using EhicBackend.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace EhicBackend.Entities
{
    public class Question : AuditBaseEntity
    {
        [Required]
        [StringLength(1000)]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string QuestionType { get; set; } = "MultipleChoice"; // MultipleChoice, TrueFalse, etc.

        [StringLength(100)]
        public string? Category { get; set; }

        [StringLength(20)]
        public string? Difficulty { get; set; } = "Medium"; // Easy, Medium, Hard

        // Navigation properties
        public virtual ICollection<AnswerChoice> AnswerChoices { get; set; } = new List<AnswerChoice>();
    }
}