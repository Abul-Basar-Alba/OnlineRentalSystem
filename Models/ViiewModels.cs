using System.ComponentModel.DataAnnotations;

namespace OnlineRentalSystem.ViewModels
{
    public class BookingViewModel
    {
        [Required]
        public Guid PropertyId { get; set; }

        // Properties for displaying property details (optional, populated by controller)
        public string PropertyTitle { get; set; }
        public string PropertyDescription { get; set; }
        public decimal PropertyPricePerDay { get; set; }
        public string PropertyImageUrl { get; set; } // URL of the main image

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Check-in Date")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Check-out Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Total Price")]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; } // Will be calculated dynamically

        // Optional: BookerId will be populated by the controller from the logged-in user
        // public string BookerId { get; set; }
    }
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        
        

        // You can add more fields here if you want them on the registration form
    }
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

}