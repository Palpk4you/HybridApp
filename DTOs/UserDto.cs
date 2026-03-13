namespace HybridApp.DTOs
{
    public class UserDto
    {
        public string? Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
    }
}