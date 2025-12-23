using EhicBackend.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace EhicBackend.Entities
{
    public class BaptismEligibility : AuditBaseEntity
    {
        public int UserId { get; set; }
        public int ExamAttemptId { get; set; }
        public bool IsEligible { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public bool CertificateIssued { get; set; } = false;
        public DateTime? CertificateIssuedAt { get; set; }
        public DateTime DeterminedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual ExamAttempt ExamAttempt { get; set; } = null!;
    }
}