using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineRentalSystem.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }

        // Navigation properties
        public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }

    // Listing model for rental items (Properties, Cars, Event Spaces, Dresses)
    public class Listing
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid OwnerId { get; set; } // Foreign key to User

        [Required]
        [StringLength(50)]
        public string Category { get; set; } // "Property", "Car", "EventSpace", "Dress"

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PricePerDay { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; } // City or address

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Category-specific properties (stored as JSON or separate columns)
        public string CategoryDetails { get; set; } // JSON for flexible attributes

        // Navigation properties
        [ForeignKey("OwnerId")]
        public virtual User Owner { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Availability> Availabilities { get; set; } = new List<Availability>();
        public virtual ICollection<ListingPhoto> Photos { get; set; } = new List<ListingPhoto>();
    }

    // Booking model for rental transactions
    public class Booking
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ListingId { get; set; } // Foreign key to Listing

        [Required]
        public Guid RenterId { get; set; } // Foreign key to User

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal TotalPrice { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } // "Pending", "Confirmed", "Cancelled", "Completed"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ListingId")]
        public virtual Listing Listing { get; set; }

        [ForeignKey("RenterId")]
        public virtual User Renter { get; set; }

        public virtual Payment Payment { get; set; }
    }

    // Review model for user feedback
    public class Review
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ListingId { get; set; } // Foreign key to Listing

        [Required]
        public Guid RenterId { get; set; } // Foreign key to User

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; } // 1 to 5 stars

        [StringLength(1000)]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ListingId")]
        public virtual Listing Listing { get; set; }

        [ForeignKey("RenterId")]
        public virtual User Renter { get; set; }
    }

    // Payment model for transactions
    public class Payment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid BookingId { get; set; } // Foreign key to Booking

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } // "Pending", "Completed", "Failed"

        [StringLength(100)]
        public string TransactionId { get; set; } // From payment gateway (e.g., Stripe)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }
    }

    // Availability model for listing availability
    public class Availability
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ListingId { get; set; } // Foreign key to Listing

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsAvailable { get; set; } = true;

        // Navigation property
        [ForeignKey("ListingId")]
        public virtual Listing Listing { get; set; }
    }

    // ListingPhoto model for storing photo URLs
    public class ListingPhoto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ListingId { get; set; } // Foreign key to Listing

        [Required]
        [StringLength(500)]
        public string PhotoUrl { get; set; } // URL to cloud storage (e.g., Azure Blob)

        [StringLength(200)]
        public string Caption { get; set; }

        // Navigation property
        [ForeignKey("ListingId")]
        public virtual Listing Listing { get; set; }
    }
}
