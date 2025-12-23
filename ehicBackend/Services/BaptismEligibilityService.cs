using EhicBackend.Data;
using EhicBackend.Entities;
using EhicBackend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EhicBackend.Services
{
    public class BaptismEligibilityService : IBaptismEligibilityService
    {
        private readonly ApplicationDbContext _context;

        public BaptismEligibilityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BaptismEligibilityDto> DetermineBaptismEligibilityAsync(int examAttemptId)
        {
            var attempt = await _context.ExamAttempts
                .Include(ea => ea.User)
                .Include(ea => ea.Exam)
                .FirstOrDefaultAsync(ea => ea.Id == examAttemptId);

            if (attempt == null)
                throw new ArgumentException("Exam attempt not found");

            // Check if eligibility record already exists
            var existingEligibility = await _context.BaptismEligibilities
                .FirstOrDefaultAsync(be => be.ExamAttemptId == examAttemptId);

            if (existingEligibility != null)
            {
                // Update existing record
                existingEligibility.IsEligible = attempt.Passed;
                existingEligibility.DeterminedAt = DateTime.UtcNow;
                existingEligibility.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Create new eligibility record
                existingEligibility = new BaptismEligibility
                {
                    UserId = attempt.UserId,
                    ExamAttemptId = examAttemptId,
                    IsEligible = attempt.Passed,
                    DeterminedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = attempt.UserId,
                    Notes = attempt.Passed ? 
                        $"Passed exam '{attempt.Exam.Title}' with {attempt.Percentage:F1}%" :
                        $"Did not pass exam '{attempt.Exam.Title}' - scored {attempt.Percentage:F1}% (required: {attempt.Exam.PassingPercentage}%)"
                };
                _context.BaptismEligibilities.Add(existingEligibility);
            }

            await _context.SaveChangesAsync();
            return await GetEligibilityWithDetailsAsync(existingEligibility.Id);
        }

        public async Task<IEnumerable<BaptismEligibilityDto>> GetEligibleStudentsAsync()
        {
            var eligibilities = await _context.BaptismEligibilities
                .Include(be => be.User)
                .Include(be => be.ExamAttempt)
                .ThenInclude(ea => ea.Exam)
                .Where(be => be.IsEligible && be.IsActive)
                .OrderByDescending(be => be.DeterminedAt)
                .ToListAsync();

            return eligibilities.Select(MapToDto);
        }

        public async Task<IEnumerable<BaptismEligibilityDto>> GetAllEligibilityRecordsAsync()
        {
            var eligibilities = await _context.BaptismEligibilities
                .Include(be => be.User)
                .Include(be => be.ExamAttempt)
                .ThenInclude(ea => ea.Exam)
                .Where(be => be.IsActive)
                .OrderByDescending(be => be.DeterminedAt)
                .ToListAsync();

            return eligibilities.Select(MapToDto);
        }

        public async Task<BaptismEligibilityDto?> GetUserEligibilityAsync(int userId)
        {
            var eligibility = await _context.BaptismEligibilities
                .Include(be => be.User)
                .Include(be => be.ExamAttempt)
                .ThenInclude(ea => ea.Exam)
                .Where(be => be.UserId == userId && be.IsActive)
                .OrderByDescending(be => be.DeterminedAt)
                .FirstOrDefaultAsync();

            return eligibility != null ? MapToDto(eligibility) : null;
        }

        public async Task<bool> IssueCertificateAsync(int eligibilityId, string? notes = null)
        {
            var eligibility = await _context.BaptismEligibilities.FindAsync(eligibilityId);
            if (eligibility == null || !eligibility.IsEligible) return false;

            eligibility.CertificateIssued = true;
            eligibility.CertificateIssuedAt = DateTime.UtcNow;
            eligibility.UpdatedAt = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(notes))
            {
                eligibility.Notes += $" | Certificate issued: {notes}";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateEligibilityAsync(int eligibilityId, bool isEligible, string? notes = null)
        {
            var eligibility = await _context.BaptismEligibilities.FindAsync(eligibilityId);
            if (eligibility == null) return false;

            eligibility.IsEligible = isEligible;
            eligibility.Notes = notes;
            eligibility.DeterminedAt = DateTime.UtcNow;
            eligibility.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<BaptismEligibilityDto> GetEligibilityWithDetailsAsync(int eligibilityId)
        {
            var eligibility = await _context.BaptismEligibilities
                .Include(be => be.User)
                .Include(be => be.ExamAttempt)
                .ThenInclude(ea => ea.Exam)
                .FirstOrDefaultAsync(be => be.Id == eligibilityId);

            if (eligibility == null)
                throw new ArgumentException("Eligibility record not found");

            return MapToDto(eligibility);
        }

        private static BaptismEligibilityDto MapToDto(BaptismEligibility eligibility)
        {
            return new BaptismEligibilityDto
            {
                Id = eligibility.Id,
                UserId = eligibility.UserId,
                UserName = $"{eligibility.User.FirstName} {eligibility.User.LastName}",
                UserEmail = eligibility.User.Email,
                ExamAttemptId = eligibility.ExamAttemptId,
                ExamTitle = eligibility.ExamAttempt.Exam.Title,
                ExamScore = eligibility.ExamAttempt.Score,
                ExamPercentage = eligibility.ExamAttempt.Percentage,
                IsEligible = eligibility.IsEligible,
                Notes = eligibility.Notes,
                CertificateIssued = eligibility.CertificateIssued,
                CertificateIssuedAt = eligibility.CertificateIssuedAt,
                DeterminedAt = eligibility.DeterminedAt
            };
        }
    }
}