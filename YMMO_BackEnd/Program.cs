using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using YMMO.Backend.API.Middleware;
using YMMO.Backend.Application.Interfaces;
using YMMO.Backend.Application.Mappings;
using YMMO.Backend.Application.Services;
using YMMO.Backend.Domain.Interfaces;
using YMMO.Backend.Domain.Repositories;
using YMMO.Backend.Infrastructure.Data;
using YMMO.Backend.Infrastructure.Data.Configurations;
using YMMO.Backend.Infrastructure.Services;
using YMMO.Backend.Infrastructure.Repositories;
using YMMO.Backend.Infrastructure.Security;

try {
var builder = WebApplication.CreateBuilder(args);

// ──────────────────────────────────────────────────────────
// 1. CONFIGURATION SERVICES & DI CONTAINER
// ──────────────────────────────────────────────────────────

    builder.Services.AddDbContext<YmmoDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IYmmoDbContext>(provider => provider.GetRequiredService<YmmoDbContext>());

    builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);
    builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
    builder.Services.AddHttpContextAccessor();

// Repositories
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IAgentRepository, AgentRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAgencyRepository, AgencyRepository>();
builder.Services.AddScoped<IOfferRepository, OfferRepository>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IWishlistItemRepository, WishlistItemRepository>();

// Services
builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IOfferService, OfferService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IAgencyService, AgencyService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserAccessor, UserAccessor>();

// PasswordHasher System (BCrypt)
builder.Services.AddScoped<IAuthentificationService, AuthentificationService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// Authentication (JWT)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
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
            NameClaimType = "username",
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ──────────────────────────────────────────────────────────
// 2. BUILD APPLICATION
// ──────────────────────────────────────────────────────────
var app = builder.Build();

// VALIDATION AUTOMAPPER (Sectionnée)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        try 
        {
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AUTO-MAPPER VALIDATION FAILED: {ex.Message}");
        }
    }
}

// ──────────────────────────────────────────────────────────
// 3. MIDDLEWARE PIPELINE
// ──────────────────────────────────────────────────────────
app.UseMiddleware<ErrorHandlingMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

}
catch (Exception ex)
{
    Console.WriteLine($"FATAL ERROR: {ex.Message}");
    Console.WriteLine($"STACK TRACE: {ex.StackTrace}");
    throw;
}