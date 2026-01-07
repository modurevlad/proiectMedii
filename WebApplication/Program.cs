using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApplication.Data;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyForJWTTokenGenerationThatIsAtLeast32CharactersLong!";
var issuer = jwtSettings["Issuer"] ?? "WebApplication";
var audience = jwtSettings["Audience"] ?? "WebApplication";

// Configure authentication - Identity uses cookies by default, JWT for API
builder.Services.AddAuthentication()
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// CORS for mobile app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMobileApp", policy =>
    {
        policy.WithOrigins("http://localhost", "https://localhost")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
        
        // For iOS simulator and device
        policy.WithOrigins("http://localhost:5000", "https://localhost:5001",
                          "http://localhost:5049", "https://localhost:7298",
                          "http://127.0.0.1:5000", "https://127.0.0.1:5001",
                          "http://127.0.0.1:5049", "https://127.0.0.1:7298")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddRazorPages(options =>
    {
        // Allow all authenticated users to access folders, but individual pages control access
        options.Conventions.AuthorizeFolder("/Appointments");
        options.Conventions.AuthorizeFolder("/Cars");
        options.Conventions.AuthorizeFolder("/Clients", "AdminOnly"); // Only Admin can access Clients
        options.Conventions.AuthorizeFolder("/Services");
        options.Conventions.AuthorizeFolder("/Mechanics");
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowMobileApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.Run();