
// --- 1. Identity/ApplicationUser.cs ---
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineRentalSystem.Models.Identity;
using OnlineRentalSystem.Models.Rental;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineRentalSystem.Models.Identity // Updated namespace
{
    // Extends IdentityUser to add custom properties for your users
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string Country { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateRegistered { get; set; } = DateTime.UtcNow;

        // Navigation property for bookings made by this user
        public ICollection<Booking> Bookings { get; set; }

        // Navigation property for properties owned/listed by this user
        public ICollection<Property> PropertiesListed { get; set; }

        // Navigation property for reviews written by this user
        public ICollection<Review> ReviewsWritten { get; set; }
    }
}


// --- 2. Rental/PropertyType.cs ---
 

namespace OnlineRentalSystem.Models.Rental // Updated namespace
{
    // Defines categories or types for rental properties (e.g., "Apartment", "Car", "Tool")
    public class PropertyType
    {
        [Key]
        public Guid PropertyTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        // Navigation property for properties belonging to this type
        public ICollection<Property> Properties { get; set; }
    }
}


// --- 3. Rental/Property.cs ---
// Updated namespace

namespace OnlineRentalSystem.Models.Rental
{
    public class Property
    {
        [Key]
        public Guid PropertyId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PricePerDay { get; set; }

        [Required]
        [StringLength(255)]
        public string Address { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(100)]
        public string StateOrProvince { get; set; }

        [Required]
        [StringLength(100)]
        public string Country { get; set; }

        [StringLength(20)]
        public string ZipCode { get; set; }

        public bool IsAvailable { get; set; } = true;

        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid PropertyTypeId { get; set; }
        [ForeignKey("PropertyTypeId")]
        public PropertyType PropertyType { get; set; }

        [Required]
        public string OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public ApplicationUser Owner { get; set; }

        [StringLength(50)]
        public string ReferenceID { get; set; }

        public ICollection<Image> Images { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<PropertyAmenity> PropertyAmenities { get; set; }
    }
}

// --- 4. Rental/Amenity.cs ---


namespace OnlineRentalSystem.Models.Rental // Updated namespace
{
    // Represents a feature or facility of a property (e.g., "WiFi", "Parking")
    public class Amenity
    {
        [Key]
        public Guid AmenityId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        // Navigation property for the junction table
        public ICollection<PropertyAmenity> PropertyAmenities { get; set; }
    }
}


// --- 5. Rental/PropertyAmenity.cs ---


namespace OnlineRentalSystem.Models.Rental // Updated namespace
{
    // Junction table for the Many-to-Many relationship between Property and Amenity
    public class PropertyAmenity
    {
        [Key]
        public Guid PropertyAmenityId { get; set; } // Surrogate key

        [Required]
        public Guid PropertyId { get; set; }
        public Property Property { get; set; }

        [Required]
        public Guid AmenityId { get; set; }
        public Amenity Amenity { get; set; }

        // You can add additional properties to the join entity if needed,
        // e.g., DateAdded, Quantity (if applicable for an amenity)
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    }
}


// --- 6. Rental/Booking.cs ---
// Updated namespace

namespace OnlineRentalSystem.Models.Rental // Updated namespace
{
    // Represents a booking/rental period for a property
    public class Booking
    {
        [Key]
        public Guid BookingId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } // e.g., "Pending", "Confirmed", "Cancelled", "Completed"

        [DataType(DataType.DateTime)]
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        // Foreign Key to the Property being booked
        public Guid PropertyId { get; set; }
        public Property Property { get; set; }

        // Foreign Key to the user who made the booking
        [Required]
        public string BookerId { get; set; } // Matches IdentityUser.Id type (string)
        public ApplicationUser Booker { get; set; }

        // Navigation property for payments related to this booking
        public ICollection<Payment> Payments { get; set; }
    }
}


// --- 7. Rental/Payment.cs ---

namespace OnlineRentalSystem.Models.Rental // Updated namespace
{
    // Represents a payment transaction for a booking
    public class Payment
    {
        [Key]
        public Guid PaymentId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } // e.g., "Pending", "Completed", "Failed", "Refunded"

        [StringLength(100)]
        public string PaymentMethod { get; set; } // e.g., "Credit Card", "PayPal", "Bank Transfer"

        [StringLength(255)]
        public string TransactionId { get; set; } // Unique ID from payment gateway

        // Foreign Key to the Booking
        public Guid BookingId { get; set; }
        public Booking Booking { get; set; }
    }
}


// --- 8. Rental/Review.cs ---
 // Updated namespace

namespace OnlineRentalSystem.Models.Rental // Updated namespace
{
    // Represents a review given by a user for a property
    public class Review
    {
        [Key]
        public Guid ReviewId { get; set; }

        [Required]
        [Range(1, 5)] // Rating out of 5 stars
        public Guid Rating { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        // Foreign Key to the Property being reviewed
        public Guid PropertyId { get; set; }
        public Property Property { get; set; }

        // Foreign Key to the user who wrote the review
        [Required]
        public string ReviewerId { get; set; } // Matches IdentityUser.Id type (string)
        public ApplicationUser Reviewer { get; set; }
    }
}


// --- 9. Rental/Image.cs ---


namespace OnlineRentalSystem.Models.Rental // Updated namespace
{
    // Stores image paths/URLs for properties
    public class Image
    {
        [Key]
        public Guid ImageId { get; set; }

        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } // URL or path to the image file

        [StringLength(255)]
        public string AltText { get; set; }

        public int DisplayOrder { get; set; } = 0; // For ordering images

        // Foreign Key to the Property this image belongs to
        public Guid PropertyId { get; set; }
        public Property Property { get; set; }
    }
}


// --- 10. Data/ApplicationDbContext.cs ---
 // Updated namespace
