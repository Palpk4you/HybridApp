namespace HybridApp.DTOs
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

        // Extra profile fields
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }

    }

}
