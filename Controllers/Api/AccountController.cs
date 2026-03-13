using HybridApp.Data;
using HybridApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HybridApp.DTOs;
namespace HybridApp.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "JwtBearer")]

    public class AccountController : ControllerBase
    {
        private readonly ICacheService _cache;
        private readonly AppDbContext _context;

        public AccountController(ICacheService cache, AppDbContext context)
        {
            _cache = cache;
            _context = context;
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDto dto)
        {
            var token = await _context.UserTokens.FirstOrDefaultAsync(t => t.Jti == dto.Jti);
            if (token != null)
            {
                token.IsRevoked = true;
                await _context.SaveChangesAsync();
                await _cache.SetAsync(dto.Jti, "revoked", TimeSpan.FromMinutes(30));
            }

            return Ok("Logged out successfully");
        }
    }

}