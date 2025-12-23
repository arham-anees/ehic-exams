namespace EhicBackend.DTOs
{
    public class InstructorDashboardDto
    {
        public int TotalQuestions { get; set; }
        public int TotalExams { get; set; }
        public int ActiveExams { get; set; }
        public int TotalStudents { get; set; }
        public int TotalAttempts { get; set; }
        public int EligibleStudents { get; set; }
        public List<ExamSummaryDto> RecentExams { get; set; } = new();
        public List<RankingDto> TopPerformers { get; set; } = new();
    }

    public class StudentDashboardDto
    {
        public string WelcomeMessage { get; set; } = string.Empty;
        public List<ExamDto> AvailableExams { get; set; } = new();
        public List<ExamAttemptDto> RecentAttempts { get; set; } = new();
        public BaptismEligibilityDto? BaptismEligibility { get; set; }
        public StudentStatsDto Stats { get; set; } = new();
    }

    public class ExamSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int TotalAttempts { get; set; }
        public decimal AverageScore { get; set; }
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }
        public bool IsOpen { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class StudentStatsDto
    {
        public int ExamsTaken { get; set; }
        public int ExamsPassed { get; set; }
        public decimal AverageScore { get; set; }
        public int CurrentRank { get; set; }
        public bool IsEligibleForBaptism { get; set; }
    }
}