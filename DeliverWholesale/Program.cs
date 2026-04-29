using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using DeliverWholesale.Application.Features.Handler.Auth;
using DeliverWholesale.Application.Interfaces;
using DeliverWholesale.Infrastructure.Services;
using DeliverWholesale.Infrastructure.Data;
using DeliverWholesale.Infrastructure.Configs;
using DeliverWholesale.API.Hubs;
using DeliverWholesale.Domain.Entities;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = null
});

// ========================
// CORS
// ========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // nécessaire pour SignalR
    });
});

// ========================
// SIGNALR
// ========================
builder.Services.AddSignalR();

// ========================
// SENDGRID
// ========================
builder.Services.Configure<SendGridSettings>(
    builder.Configuration.GetSection("SendGridSettings"));

// ========================
// MEDIATR
// ========================
builder.Services.AddMediatR(typeof(LoginHandler).Assembly);

// ========================
// DATABASE
// ========================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// ========================
// JWT CONFIG
// ========================
builder.Services.Configure<JwtConfig>(
    builder.Configuration.GetSection("JwtConfig"));

var jwt = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt!.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt.Secret)
            ),
            ClockSkew = TimeSpan.Zero
        };

        // Support SignalR (token dans query string)
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/hubs/notifications"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// ========================
// SERVICES
// ========================

// ========================
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<PricingService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<StockService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<NotificationService>();

// ========================
// CONTROLLERS + JSON
// ========================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });

// ========================
// SWAGGER
// ========================
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DeliverWholesale API",
        Version = "v1",
        Description = "Backend API for DeliverWholesale platform"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

// ========================
// BUILD APP
// ========================
var app = builder.Build();

// ========================
// MIDDLEWARE PIPELINE
// ========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DeliverWholesale API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

// CORS doit être avant Authentication/Authorization
app.UseCors("AngularPolicy");

app.UseAuthentication();

// ========================
// MIDDLEWARE BLACKLIST JWT (Logout)
// ========================
app.Use(async (context, next) =>
{
    var token = context.Request.Headers["Authorization"]
                       .ToString().Replace("Bearer ", "").Trim();

    if (!string.IsNullOrEmpty(token))
    {
        var jwtService = context.RequestServices.GetRequiredService<JwtService>();
        if (jwtService.IsTokenRevoked(token))
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                "{\"message\": \"Token révoqué. Veuillez vous reconnecter.\"}");
            return;
        }
    }

    await next();
});

app.UseAuthorization();

app.MapHub<NotificationHub>("/hubs/notifications");
app.MapControllers();

// ========================
// AUTO MIGRATION + DEFAULT ADMIN
// ========================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    db.Database.Migrate();

    if (!db.Users.Any(u => u.Role == Role.Admin))
    {
        db.Users.Add(new User
        {
            Nom = "Admin",
            Prenom = "System",
            Email = "admin@admin.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
            Role = Role.Admin,
            EmailConfirmationToken = null,
            IsEmailConfirmed = true
        });

        db.SaveChanges();
        Console.WriteLine("✅ Admin créé : admin@admin.com / Admin123");
    }
}

app.Run();