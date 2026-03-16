using AutoMapper;
using HybridApp.Api.Controllers;
using HybridApp.Data; // for Role entity
using HybridApp.DTOs;
using HybridApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;


namespace HybridApp.Controllers.Web
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class RolesController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;

        public RolesController(ILogger<AuthController> logger, IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: Roles
        public async Task<IActionResult> Index()
        {
            //var roles = await _roleService.GetAllRolesAsync();
            //var roleDtos = _mapper.Map<List<RoleDto>>(roles);
            //return View(roleDtos);
            var roles = await _roleService.GetAllRolesAsync();
            var vm = new RoleIndexViewModel
            {
                Roles = roles,
                NewRole = new RoleDto()
            };
            return View(vm);

        }

        // GET: Roles/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null) return NotFound();
            var dto = _mapper.Map<RoleDto>(role);
            _logger.LogInformation("Role Dto detail", dto);
            return View(dto);
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleIndexViewModel model)
        {
            //if (!ModelState.IsValid) return View(dto);

            //var role = _mapper.Map<Role>(dto);
            //var roledto = _mapper.Map<RoleDto>(role);
            //var result =  await _roleService.CreateRoleAsync(roledto);

            //if (!result.Succeeded)
            //{
            //    foreach (var error in result.Errors)
            //    {
            //        ModelState.AddModelError("", error.Description);
            //    }
            //    return View(dto);
            //}
            var dto = model.NewRole;

            if (!ModelState.IsValid)
            {
                var roles = await _roleService.GetAllRolesAsync();
                var vm = new RoleIndexViewModel { Roles = roles, NewRole = dto };
                return View("Index", vm);
            }

            var result = await _roleService.CreateRoleAsync(dto);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                var roles = await _roleService.GetAllRolesAsync();
                var vm = new RoleIndexViewModel { Roles = roles, NewRole = dto };
                return View("Index", vm);
            }

            return RedirectToAction(nameof(Index));


        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(RoleDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var result = await _roleService.UpdateRoleAsync(dto);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }



        // POST: Roles/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Delete(string id)
        //{
        //    await _roleService.DeleteRoleAsync(id);
        //    return RedirectToAction(nameof(Index));
        //}
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _roleService.DeleteRoleAsync(id);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                // reload roles and return Index with errors
                var roles = await _roleService.GetAllRolesAsync();
                var vm = new RoleIndexViewModel { Roles = roles, NewRole = new RoleDto() };
                return View("Index", vm);
            }

            return RedirectToAction(nameof(Index));
        }

    }
}