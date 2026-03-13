namespace HybridApp.DTOs
{
    public class RoleDto
    {
        public string? Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public string? NormalizedName { get; set; }
    }
}