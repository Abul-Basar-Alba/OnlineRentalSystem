using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }[HttpGet]
        public IActionResult Admin()
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
        [HttpPost]
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
        // GET: /YourRoleManagementController/AssignRoleToUser
        // This action will display the form to assign roles
        [HttpGet]
        public async Task<IActionResult> AssignRoleToUser()
        {
            // Fetch all users and populate ViewBag.Users
            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = users.Select(u => new SelectListItem
            {
                Value = u.Id,     // The actual value sent to the controller
                Text = u.UserName // What the user sees in the dropdown
            }).ToList();

            // Fetch all roles and populate ViewBag.Roles (useful if you want a role dropdown too)
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();

            // Optionally set a default message or clear previous messages
            ViewBag.Message = "";
            ViewBag.IsSuccess = false;

            return View();
        }

        // POST: /YourRoleManagementController/AssignRoleToUser
        // This action will handle the form submission to assign the role
        [HttpPost]
        [ValidateAntiForgeryToken] // Good practice for preventing cross-site request forgery attacks
        public async Task<IActionResult> AssignRoleToUser(string userId, string roleName)
        {
            // --- Important: Re-populate ViewBag.Users and ViewBag.Roles here ---
            // This is crucial because if validation fails, you'll return to the same view,
            // and the dropdowns need their data populated again.
            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = users.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.UserName
            }).ToList();

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();
            // --- End of re-population ---


            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleName))
            {
                ViewBag.Message = "Please select both a User and a Role.";
                ViewBag.IsSuccess = false;
                return View(); // Return the view with error message
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.Message = $"User not found with ID: '{userId}'.";
                ViewBag.IsSuccess = false;
                return View();
            }

            // Check if the role actually exists
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                ViewBag.Message = $"Role '{roleName}' does not exist.";
                ViewBag.IsSuccess = false;
                return View();
            }

            // Check if the user already has the role to avoid errors and unnecessary operations
            if (await _userManager.IsInRoleAsync(user, roleName))
            {
                ViewBag.Message = $"User '{user.UserName}' is already in role '{roleName}'.";
                ViewBag.IsSuccess = false;
                return View();
            }

            // Attempt to add the user to the role
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                Console.WriteLine($"User '{user.UserName}' successfully assigned to role '{roleName}'.");
                ViewBag.Message = $"User '{user.UserName}' assigned to role '{roleName}' successfully!";
                ViewBag.IsSuccess = true;
                // Optionally, you might want to redirect to a user list or role list page
                // return RedirectToAction("Index", "Users");
                return View(); // Stay on the same page, showing success
            }
            else
            {
                // If there are errors during assignment, display them
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                Console.WriteLine($"Error assigning role to user: {errors}");
                ViewBag.Message = $"Failed to assign role to user: {errors}";
                ViewBag.IsSuccess = false;
                return View(); // Stay on the same page, showing error
            }
        }
    }
}
