// EMS.API/Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using EMS.DAL.Data;
using EMS.DAL.Repository;
using EMS.Services.Helpers;
using EMS.Services.Interfaces;
using EMS.Services.Implementations;
using EMS.API.Middleware;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configure URLs from environment or use defaults
var urls = builder.Configuration["ASPNETCORE_URLS"] ?? "http://localhost:5000;https://localhost:5001";
builder.WebHost.UseUrls(urls.Split(";"));

// Configuration
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");
var jwtSettings = configuration.GetSection("Jwt");
var jwtSecret = jwtSettings["Secret"];
var jwtIssuer = jwtSettings["Issuer"];
var jwtAudience = jwtSettings["Audience"];
var jwtExpirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

// Database
builder.Services.AddDbContext<EMSContext>(options =>
    options.UseSqlServer(connectionString));

// Repositories (Project-2)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<ISpeakerRepository, SpeakerRepository>();
builder.Services.AddScoped<IParticipantEventRepository, ParticipantEventRepository>();

// JWT Helper
builder.Services.AddSingleton(new JwtTokenHelper(jwtSecret, jwtIssuer, jwtAudience, jwtExpirationMinutes));

// Services (Project-3)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ISpeakerService, SpeakerService>();
builder.Services.AddScoped<IParticipantService, ParticipantService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<PasswordHasher>();

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true
    };
});

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// CORS
var corsOrigins = builder.Configuration["CORS:AllowedOrigins"]?.Split(",") ?? new[] { "*" };
var isDevelopment = builder.Environment.IsDevelopment();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        if (isDevelopment)
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
        else
        {
            // Production: use configured origins
            if (corsOrigins.Contains("*"))
            {
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }
            else
            {
                policy.WithOrigins(corsOrigins).AllowAnyMethod().AllowAnyHeader();
            }
        }
    });
});

// Caching
builder.Services.AddMemoryCache();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EMS Virtual Event Management System API",
        Version = "v1.0"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Swagger - only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EMS API v1"));
}

// HTTPS redirect - enable in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

// Database migration
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EMSContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<PasswordHasher>();

    context.Database.Migrate();
    Console.WriteLine("✅ Database ready");

    // Runtime admin seeding - create or update admin user
    var existingAdmin = context.UserInfos.FirstOrDefault(u => u.EmailId == "admin@upgrad.com");
    
    if (existingAdmin == null)
    {
        // No admin exists - create one
        var adminUser = new EMS.DAL.Models.UserInfo
        {
            EmailId = "admin@upgrad.com",
            UserName = "Admin User",
            Role = "Admin",
            Password = passwordHasher.HashPassword("Admin@321"),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    
        context.UserInfos.Add(adminUser);
        context.SaveChanges();
        Console.WriteLine(" Default admin user created");
    }
    else
    {
        // Admin exists - check if password needs to be hashed
        // Plain text password "Admin@321" won't verify against itself as a hash
        if (!passwordHasher.VerifyPassword("Admin@321", existingAdmin.Password))
        {
            // Password is not properly hashed - update it
            existingAdmin.Password = passwordHasher.HashPassword("Admin@321");
            existingAdmin.Role = "Admin"; 
            existingAdmin.IsActive = true; 
            context.SaveChanges();
            Console.WriteLine(" Admin password updated to hashed format");
        }
        else
        {
            Console.WriteLine(" Admin user already exists with correct password");
        }
    }
}


Console.WriteLine(" EMS API Server is running!");
Console.WriteLine($"  Swagger UI → http://localhost:5000/swagger (Development only)");
Console.WriteLine($"  Health     → http://localhost:5000/health");
Console.WriteLine($"  Environment: {builder.Environment.EnvironmentName}");

app.Run();