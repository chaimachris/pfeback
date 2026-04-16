using DeliverWholesale.Data;
using DeliverWholesale.Helpers;
using DeliverWholesale.Models;
using DeliverWholesale.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MediatR;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;



var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = null
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ========================
// MEDIATR
// ========================
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

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
    });

builder.Services.AddAuthorization();

// ========================
// SERVICES
// ========================
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<PricingService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<StockService>();

// ========================
// CONTROLLERS + JSON + SWAGGER
// ========================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });

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
// MIDDLEWARE
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

app.UseAuthentication();
app.UseAuthorization();

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
            Role = Role.Admin
        });

        db.SaveChanges();
         Console.WriteLine("Admin créé : admin@admin.com / Admin123");
    }
}
app.UseCors("AngularPolicy");

app.Run();