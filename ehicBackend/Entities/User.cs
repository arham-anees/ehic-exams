using EhicBackend.Entities.Common;

namespace EhicBackend.Entities
{
    public class User: BaseEntity
    {
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Email { get; set; } = null!;

        public String FirstName { get; set; } = null!;
        public String LastName { get; set; } = null!;
        public Role Role { get; set; }
    }
}