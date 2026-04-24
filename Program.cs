using ExaminationSystem.Common.Data;
using ExaminationSystem.Common.Extension;
using ExaminationSystem.Common.Middleware;
using ExaminationSystem.Common.Models;
using ExaminationSystem.Common.Repositories;
using ExaminationSystem.Common.Services;
using FluentValidation;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.OpenApi;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//Custom extension registrations
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorizationPolicies();
builder.Services.AddEmailSettings(builder.Configuration);
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IEmailService, MockEmailService>();
}
// builder.Services.AddSwagger();
// Fluent_validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Identity
builder.Services.AddIdentityCore<AppUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<SignInManager<AppUser>>();
builder.Services.AddHttpContextAccessor();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Email Confirmation Helper
builder.Services.AddScoped<IEmailConfirmationHelper, EmailConfirmationHelper>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});
builder.Services.AddOpenApi();
var app = builder.Build();
// Global exception middleware handler 
app.UseMiddleware<GlobalExceptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/", () => Results.Ok(new
{
    message = "ExaminationSystem API is running.",
    openApi = app.Environment.IsDevelopment() ? "/openapi/v1.json" : null
}));

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
if (app.Environment.IsDevelopment())
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ExaminationSystem.Common.Data.AppDbContext>();
        if (db.Database.IsRelational())
        {
            await db.Database.MigrateAsync();
        }
    }
    catch
    {
        Console.WriteLine("Migrations already applied or tables exist.");
    }
}

app.Run();

public partial class Program { }
