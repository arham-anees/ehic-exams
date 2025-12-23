using EhicBackend.Entities.Common;

namespace EhicBackend.Entities
{
    public class User: BaseEntity
    {
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Email { get; set; } = null!;

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}