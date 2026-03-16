using System.ComponentModel.DataAnnotations;

namespace HybridApp.DTOs
{
    public class RoleDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        [StringLength(50, ErrorMessage = "Role name cannot exceed 50 characters")]
        public string Name { get; set; }

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? Description { get; set; }
        
        public string? NormalizedName { get; set; }
    }
}