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

            // Configure the many-to-many relationship for Property and Amenity
            // using the PropertyAmenity junction table.
            modelBuilder.Entity<PropertyAmenity>()
                .HasKey(pa => pa.PropertyAmenityId); // Use a surrogate key for the join table

            modelBuilder.Entity<PropertyAmenity>()
                .HasOne(pa => pa.Property)
                .WithMany(p => p.PropertyAmenities)
                .HasForeignKey(pa => pa.PropertyId);

            modelBuilder.Entity<PropertyAmenity>()
                .HasOne(pa => pa.Amenity)
                .WithMany(a => a.PropertyAmenities)
                .HasForeignKey(pa => pa.AmenityId);

            // Configure relationships for ApplicationUser
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.Booker)
                .HasForeignKey(b => b.BookerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete on user deletion if bookings exist

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.PropertiesListed)
                .WithOne(p => p.Owner)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete on user deletion if properties exist

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.ReviewsWritten)
                .WithOne(r => r.Reviewer)
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete on user deletion if reviews exist

            // Configure Property to PropertyType relationship
            modelBuilder.Entity<Property>()
                .HasOne(p => p.PropertyType)
                .WithMany(pt => pt.Properties)
                .HasForeignKey(p => p.PropertyTypeId)
                .OnDelete(DeleteBehavior.Restrict); // Or .Cascade, depending on your business logic

            // Configure Booking to Property relationship
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Property)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.PropertyId)
                .OnDelete(DeleteBehavior.Restrict); // Or .Cascade

            // Configure Payment to Booking relationship
            modelBuilder.Entity<Payment>()
                .HasOne(py => py.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(py => py.BookingId)
                .OnDelete(DeleteBehavior.Cascade); // Payments should be deleted if booking is deleted

            // Configure Review to Property relationship
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Property)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.PropertyId)
                .OnDelete(DeleteBehavior.Cascade); // Reviews might be deleted if property is deleted

            // Configure Image to Property relationship
            modelBuilder.Entity<Image>()
                .HasOne(i => i.Property)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.PropertyId)
                .OnDelete(DeleteBehavior.Cascade); // Images should be deleted if property is deleted

            // If you have specific table names you want to enforce for Identity tables,
            // you can do it here. By default, IdentityDbContext creates tables like AspNetUsers, etc.
            // For example:
            // modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            // modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            // etc.
        }
    }
}