using Autofac;
using Autofac.Extensions.DependencyInjection;
using HybridApp.Data;
using HybridApp.Extensions;
using HybridApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add MVC + Razor
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// CORS for frontend (adjust origin as needed)
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Serilog logging
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7
    )
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity with roles
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Cookie paths (Identity.Application scheme is already wired by AddIdentity)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// HybridCache
builder.Services.AddHybridCache();
builder.Services.AddScoped<ICacheService, HybridCacheService>();

// JWT (API login)
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
    throw new InvalidOperationException("JWT Key is missing in configuration.");

//builder.Services.AddAuthentication() // don’t override Identity’s cookie scheme
//    .AddJwtBearer("JwtBearer", options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
//        };

//        options.Events = new JwtBearerEvents
//        {
//            OnTokenValidated = async context =>
//            {
//                var cache = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
//                var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
//                var jti = context.Principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

//                var cached = await cache.GetAsync(jti);
//                if (cached == "revoked")
//                {
//                    context.Fail("Token revoked");
//                    return;
//                }

//                if (cached == null)
//                {
//                    var tokenInDb = await db.UserTokens.FirstOrDefaultAsync(t => t.Jti == jti && !t.IsRevoked);
//                    if (tokenInDb == null)
//                    {
//                        context.Fail("Token not found");
//                        return;
//                    }

//                    await cache.SetAsync(jti, "active", TimeSpan.FromMinutes(30));
//                }
//            }
//        };
//    });
builder.Services.AddJwtAuthentication(builder.Configuration);
// AutoMapper
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

// Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(container =>
{
    container.RegisterModule(new RepositoryModule());
    container.RegisterModule(new ServiceModule());
});

// Email
//builder.Services.AddTransient<IEmailSender, HybridApp.Services.EmailSender>(); //Now using EmailService with MailKit and register view Ioc Continer

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Apply CORS only for APIs
app.UseCors("FrontendPolicy");

// ✅ Correct order: Authentication first, then Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();