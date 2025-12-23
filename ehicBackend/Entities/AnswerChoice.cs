using EhicBackend.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace EhicBackend.Entities
{
    public class AnswerChoice : BaseEntity
    {
        public int QuestionId { get; set; }

        [Required]
        [StringLength(500)]
        public string ChoiceText { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }

        public int DisplayOrder { get; set; }

        // Navigation properties
        public virtual Question Question { get; set; } = null!;
    }
}