using AutoMapper;
using HybridApp.DTOs;
using HybridApp.Data; // for Role entity
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HybridApp.Controllers.Web
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class RolesController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public RolesController(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        // GET: Roles
        public async Task<IActionResult> Index()
        {
            var roles = await _roleService.GetAllRolesAsync();
            var roleDtos = _mapper.Map<List<RoleDto>>(roles);
            return View(roleDtos);
        }

        // GET: Roles/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null) return NotFound();

            var dto = _mapper.Map<RoleDto>(role);
            return View(dto);
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var role = _mapper.Map<Role>(dto);
            var roledto = _mapper.Map<RoleDto>(role);
            await _roleService.CreateRoleAsync(roledto);

            return RedirectToAction(nameof(Index));

        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(RoleDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var role = await _roleService.GetRoleByIdAsync(dto.Id);
            if (role == null) return NotFound();

            // Map updated fields from DTO back into entity
            _mapper.Map(dto, role);
            await _roleService.UpdateRoleAsync(role);

            return RedirectToAction(nameof(Index));
        }

        // POST: Roles/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            await _roleService.DeleteRoleAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}