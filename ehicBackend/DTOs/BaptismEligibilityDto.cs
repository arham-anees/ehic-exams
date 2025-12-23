namespace EhicBackend.DTOs
{
    public class BaptismEligibilityDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public int ExamAttemptId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public int ExamScore { get; set; }
        public decimal ExamPercentage { get; set; }
        public bool IsEligible { get; set; }
        public string? Notes { get; set; }
        public bool CertificateIssued { get; set; }
        public DateTime? CertificateIssuedAt { get; set; }
        public DateTime DeterminedAt { get; set; }
    }

    public class UpdateEligibilityDto
    {
        public bool IsEligible { get; set; }
        public string? Notes { get; set; }
    }

    public class IssueCertificateDto
    {
        public string? Notes { get; set; }
    }
}