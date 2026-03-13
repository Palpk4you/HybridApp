namespace HybridApp.Data.Entities
{
    public class UserToken
    {
        public int Id { get; set; }
        public string Jti { get; set; }          // Unique ID from JWT
        public string UserId { get; set; }       // FK to Identity User
        public DateTime Expires { get; set; }    // Expiry of token
        public bool IsRevoked { get; set; }      // Revoked on logout
        public string TokenType { get; set; }    // "Access" or "Refresh"
    }
}
