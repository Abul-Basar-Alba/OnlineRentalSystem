using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineRentalSystem.Models.Identity; // Your custom ApplicationUser model
using OnlineRentalSystem.ViewModels; // We'll define these ViewModels below

namespace OnlineRentalSystem.Controllers
{
    public class AccountController : Controller // Conventionally, Identity-related controllers are named AccountController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager; // For managing roles

        // Constructor for dependency injection
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager) // Inject RoleManager
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken] // Protects against Cross-Site Request Forgery (CSRF)
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email, // Email is commonly used as UserName
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    // You can add more properties as needed, e.g., Address, City, etc.
                    Address = model.Address, // Assuming you have these properties in your ViewModel
                    City = model.City,
                    Country = model.Country,

                    // Additional custom properties can be set here
                    DateRegistered = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Optional: Assign a default role to new users, e.g., "Renter"
                    // You should ensure this role exists in your database first,
                    // perhaps through a seeding mechanism or admin UI.
                    if (!await _roleManager.RoleExistsAsync("Renter"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Renter"));
                    }
                    await _userManager.AddToRoleAsync(user, "Renter");


                    // Sign in the user immediately after successful registration
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    Console.WriteLine($"User {user.Email} registered successfully and signed in.");
                    return RedirectToAction("Index", "Home"); // Redirect to home page or a success page
                }

                // If registration failed, add errors to ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    Console.WriteLine($"Registration error: {error.Description}");
                }
            }

            // If ModelState is not valid or creation failed, return to the view with errors
            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl; // Store return URL for redirection after login
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Attempt to sign in the user
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false); // Set to true to lock out user after multiple failed attempts

                if (result.Succeeded)
                {
                    Console.WriteLine($"User {model.Email} logged in successfully.");
                    // Redirect to the original URL if provided, otherwise to home
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    // Handle two-factor authentication if enabled for the user
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    Console.WriteLine($"User {model.Email} account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    Console.WriteLine($"Invalid login attempt for user {model.Email}.");
                    return View(model);
                }
            }

            // If ModelState is not valid, return to the view with errors
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            Console.WriteLine("User logged out.");
            return RedirectToAction("Index", "Home"); // Redirect to home page
        }

        // GET: /Account/Lockout (Placeholder for locked out accounts)
        [HttpGet]
        public IActionResult Lockout()
        {
            return View();
        }

        // GET: /Account/LoginWith2fa (Placeholder for 2FA)
        [HttpGet]
        public IActionResult LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Logic for 2FA login (e.g., display a code input field)
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["RememberMe"] = rememberMe;
            return View();
        }

        // Helper method for safe redirection
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // --- Role Management Examples (Conceptual - you'd build UI/APIs around these) ---

        // Example: Create a new role
        [HttpGet]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Role name cannot be empty.");
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                {
                    Console.WriteLine($"Role '{roleName}' created successfully.");
                    // Redirect to a role management view or success message
                    return Ok($"Role '{roleName}' created.");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error creating role: {error.Description}");
                    }
                    return BadRequest("Failed to create role.");
                }
            }
            return Ok($"Role '{roleName}' already exists.");
        }

        // Example: Assign a role to a user
        [HttpGet]
        public async Task<IActionResult> AssignRoleToUser(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID '{userId}' not found.");
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return NotFound($"Role '{roleName}' not found.");
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                Console.WriteLine($"User '{user.Email}' assigned to role '{roleName}'.");
                return Ok($"User '{user.Email}' assigned to role '{roleName}'.");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error assigning role: {error.Description}");
                }
                return BadRequest("Failed to assign role.");
            }
        }
    }
}
