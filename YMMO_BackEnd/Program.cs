using Microsoft.EntityFrameworkCore;
using YMMO.Backend.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();