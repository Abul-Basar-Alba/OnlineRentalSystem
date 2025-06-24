using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineRentalSystem.Data;
using OnlineRentalSystem.Models;
using OnlineRentalSystem.Models.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure the database connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register your ApplicationDbContext with the dependency injection container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)); // Use .UseSqlite() if you prefer SQLite, or .UsePostgreSql(), etc.

// Add database developer page exception filter (useful for development environment)
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure ASP.NET Core Identity
// Uses ApplicationUser as your user type and ApplicationDbContext as the store
builder.Services.AddIdentity<ApplicationUser,IdentityRole>(options => {
    // Configure Identity options here if needed, e.g., password requirements
   /* options.SignIn.RequireConfirmedAccount = true; */// Example: Require email confirmation
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredUniqueChars = 1;
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>(); // Specifies ApplicationDbContext as the store for Identity

// Add MVC controllers with views support
builder.Services.AddControllersWithViews();

// Add Razor Pages support (often used for Identity UI)
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // In development, show database errors on a dedicated page
    app.UseMigrationsEndPoint();
}
else
{
    // In production, handle exceptions and use HSTS
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseStaticFiles(); // Enable serving static files (CSS, JS, images)

app.UseRouting(); // Enables routing middleware

// Authentication and Authorization middleware. Order is important: Authentication before Authorization.
app.UseAuthentication();
app.UseAuthorization();

// Map MVC controllers and Razor Pages
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // Default route for MVC
app.MapRazorPages(); // Maps Razor Pages endpoints

app.Run(); // Starts the application