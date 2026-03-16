namespace HybridApp.Data.Entities
{
    public class UserProfile
    {
        public int Id { get; set; }

        // Foreign key to Identity user
        public string UserId { get; set; }
        public User User { get; set; }

        // Extra fields
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
    }
}
