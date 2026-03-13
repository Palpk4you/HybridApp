using System.ComponentModel.DataAnnotations;

namespace HybridApp.DTOs
{
    public class LogoutDto
    {
        [Required]
        public string Jti { get; set; }
    }
}
