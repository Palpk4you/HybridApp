using HybridApp.DTOs;

namespace HybridApp.ViewModels
{
    public class RoleIndexViewModel
    {
        public IEnumerable<RoleDto> Roles { get; set; } = new List<RoleDto>();
        public RoleDto NewRole { get; set; } = new RoleDto();
    }

}


/*
 *Why We Use ViewModels
- Separation of concerns: Entities represent DB tables, DTOs represent transfer objects for services/APIs, and ViewModels represent what the UI needs. Each layer stays clean.
- Different data types in one view: A single view often needs multiple sources of data (e.g., a list of roles + a form for a new role). A ViewModel lets you package them together.
- Avoid over-posting: You only expose the properties the view should bind to, not the entire entity.
- Strong typing: Instead of relying on ViewBag or dynamic, you get compile-time safety and IntelliSense in Razor views.

🔹 When to Use ViewModels
- When a view needs multiple models at once (like your Index needing both a list of roles and a new role form).
- When you want to combine entity data with extra UI state (e.g., dropdown lists, flags, calculated values).
- When you want to simplify binding (e.g., flatten nested objects for easier form posting).

 */