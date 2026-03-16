using HybridApp.Data;
using HybridApp.Data.Entities;
using HybridApp.DTOs;
using HybridApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;


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
        private readonly IWebHostEnvironment _env;
        private readonly IEmailService _emailService;

        public AuthController(ILogger<AuthController> logger,
                                UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IJwtService jwtService,
                              AppDbContext context,
                              ICacheService cache, IWebHostEnvironment env, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _context = context;
            _cache = cache;
            _logger = logger;
            _env = env;
            _emailService = emailService;   
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var user = new User { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Assign default role "Guest"
            if (!await _userManager.IsInRoleAsync(user, "Guest"))
            {
                await _userManager.AddToRoleAsync(user, "Guest");
            }

            // Optionally send email confirmation
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Auth",
                new { userId = user.Id, token }, Request.Scheme);

            _logger.LogInformation("Email confirmation link: {Link}", confirmationLink);
            // TODO: send via email service

            // 🔹 Load HTML template
            var templatePath = Path.Combine(_env.ContentRootPath, "Templates", "ConfirmEmail.html");
            var template = await System.IO.File.ReadAllTextAsync(templatePath);

            // 🔹 Replace placeholders
            template = template.Replace("{{FirstName}}", user.UserName)
                               .Replace("{{ConfirmationLink}}", confirmationLink);

            // 🔹 Send email
            await _emailService.SendEmailAsync(user.Email, "Confirm your email", template);

            return Ok("User registered successfully with Guest role");

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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return BadRequest("User not found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Auth",
                new { userId = user.Id, token }, Request.Scheme);

        

            _logger.LogInformation("Password reset link: {Link}", resetLink);
            // TODO: send via email service
            await _emailService.SendEmailAsync(
                    dto.Email,
                    "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(resetLink)}'>clicking here</a>.");


            return Ok("Password reset link generated");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return BadRequest("User not found");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok("Password reset successful");
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest("User not found");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok("Email confirmed successfully");
        }

    }
}