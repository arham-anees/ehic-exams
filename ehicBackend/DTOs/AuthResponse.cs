namespace EhicBackend.DTOs
{
    public class AuthResponse
    {
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
        public UserDto User { get; set; } = null!;
    }
}