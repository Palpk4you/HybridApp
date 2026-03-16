using HybridApp.Data;
using HybridApp.Data.Entities;
using HybridApp.DTOs;
using HybridApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HybridApp.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly AppDbContext _context;
        private readonly ICacheService _cache;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger,
                                UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IJwtService jwtService,
                              AppDbContext context,
                              ICacheService cache)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _context = context;
            _cache = cache;
            _logger = logger;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var user = new User { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized();

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            var jwtResult = _jwtService.GenerateToken(user, roles);
            var token = jwtResult.Token;
            var jti = jwtResult.Jti;


            var userToken = new UserToken
            {
                Jti = jti,
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddMinutes(30),
                IsRevoked = false,
                TokenType = "Access"
            };

            _context.UserTokens.Add(userToken);
            await _context.SaveChangesAsync();

            await _cache.SetAsync(jti, "active", TimeSpan.FromMinutes(30));


            var value = await _cache.GetAsync(jti);
            _logger.LogInformation("Cache lookup for {Jti} returned {Value}", jti, value);

            return Ok(new TokenResponseDto { AccessToken = token, Jti = jti });
        }



        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDto dto)
        {
            var value = await _cache.GetAsync(dto.Jti);
            _logger.LogInformation("Cache lookup for {Jti} returned {Value}", dto.Jti, value);

            var token = await _context.UserTokens.FirstOrDefaultAsync(t => t.Jti == dto.Jti);
            if (token != null)
            {
                token.IsRevoked = true;
                await _context.SaveChangesAsync();
                await _cache.RemoveAsync(dto.Jti);
                _logger.LogInformation("Token {Jti} revoked and removed from cache", dto.Jti);

            }
            return Ok("Logged out successfully");
        }
    }
}