using EhicBackend.DTOs;

namespace EhicBackend.Services
{
    public interface IBaptismEligibilityService
    {
        Task<BaptismEligibilityDto> DetermineBaptismEligibilityAsync(int examAttemptId);
        Task<IEnumerable<BaptismEligibilityDto>> GetEligibleStudentsAsync();
        Task<IEnumerable<BaptismEligibilityDto>> GetAllEligibilityRecordsAsync();
        Task<BaptismEligibilityDto?> GetUserEligibilityAsync(int userId);
        Task<bool> IssueCertificateAsync(int eligibilityId, string? notes = null);
        Task<bool> UpdateEligibilityAsync(int eligibilityId, bool isEligible, string? notes = null);
    }
}