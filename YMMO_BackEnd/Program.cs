using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using YMMO.Backend.API.Middleware;
using YMMO.Backend.Application.Interfaces;
using YMMO.Backend.Application.Services;
using YMMO.Backend.Domain.Interfaces;
using YMMO.Backend.Infrastructure.Data;
using YMMO.Backend.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);


// ==========================================
// GESTION DU CRYPTAGE DU MDP (BCrypt)
// ==========================================
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuthentificationService, AuthentificationService>();

// ==========================================
// CONFIGURATION AUTHENTIFICATION JWT STRICT
// ==========================================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ClockSkew = TimeSpan.Zero,
        NameClaimType = "username"
    };
});


// ─────────────────────────────────────────
// 1. SERVICES REGISTRATION (DI Container)
// ─────────────────────────────────────────

// Database context — registers YmmoDbContext with PostgreSQL
builder.Services.AddDbContext<YmmoDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// API Controllers
builder.Services.AddControllers();

// Swagger/OpenAPI — for API documentation and manual testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

    
// ─────────────────────────────────────────
// 2. BUILD THE APPLICATION
// ─────────────────────────────────────────

var app = builder.Build();

// ─────────────────────────────────────────
// 3. MIDDLEWARE PIPELINE
// ─────────────────────────────────────────

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>(); // Need to be 1st Position
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();