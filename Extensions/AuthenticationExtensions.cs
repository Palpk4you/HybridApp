namespace HybridApp.Extensions
{
    using HybridApp.Data;
    using HybridApp.Services;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.EntityFrameworkCore;
    using HybridApp.Models;

    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtKey = configuration["Jwt:Key"];

            services.AddAuthentication()
                .AddJwtBearer("JwtBearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var cache = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
                            var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                            var jti = context.Principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                            var cached = await cache.GetAsync(jti);
                            if (cached == "revoked")
                            {
                                context.Fail("Token revoked");
                                return;
                            }

                            if (cached == null)
                            {
                                var tokenInDb = await db.UserTokens.FirstOrDefaultAsync(t => t.Jti == jti && !t.IsRevoked);
                                if (tokenInDb == null)
                                {
                                    context.Fail("Token not found");
                                    return;
                                }

                                await cache.SetAsync(jti, "active", TimeSpan.FromMinutes(30));
                            }
                        }
                    };
                });

            return services;
        }
    }
}
