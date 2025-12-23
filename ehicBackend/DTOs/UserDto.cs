namespace EhicBackend.DTOs
{
    public class UserDto
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;

        public String FirstName { get; set; } = null!;
        public String LastName { get; set; } = null!;
        public int RoleId { get; set; }
        public string Role { get; set; } = null!;
    }
}