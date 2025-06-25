using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineRentalSystem.Models;
using OnlineRentalSystem.Models.Identity;
using OnlineRentalSystem.Models.Rental;

namespace OnlineRentalSystem.Data // Updated namespace
{
    // ApplicationDbContext extends IdentityDbContext to include Identity tables
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for your custom rental entities
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<PropertyAmenity> PropertyAmenities { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure unique index for ReferenceID
            modelBuilder.Entity<Property>()
                .HasIndex(p => p.ReferenceID)
                .IsUnique()
                .HasFilter("[ReferenceID] IS NOT NULL");

            // Configure PropertyAmenity
            modelBuilder.Entity<PropertyAmenity>()
                .HasKey(pa => pa.PropertyAmenityId);

            modelBuilder.Entity<PropertyAmenity>()
                .HasOne(pa => pa.Property)
                .WithMany(p => p.PropertyAmenities)
                .HasForeignKey(pa => pa.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PropertyAmenity>()
                .HasOne(pa => pa.Amenity)
                .WithMany(a => a.PropertyAmenities)
                .HasForeignKey(pa => pa.AmenityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ApplicationUser relationships
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.Booker)
                .HasForeignKey(b => b.BookerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.PropertiesListed)
                .WithOne(p => p.Owner)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.ReviewsWritten)
                .WithOne(r => r.Reviewer)
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Property to PropertyType
            modelBuilder.Entity<Property>()
                .HasOne(p => p.PropertyType)
                .WithMany(pt => pt.Properties)
                .HasForeignKey(p => p.PropertyTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Booking to Property
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Property)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Payment to Booking
            modelBuilder.Entity<Payment>()
                .HasOne(py => py.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(py => py.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Review to Property
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Property)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Image to Property
            modelBuilder.Entity<Image>()
                .HasOne(i => i.Property)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}