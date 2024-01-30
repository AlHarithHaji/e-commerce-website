#nullable disable
using He_SheStore.Areas.Identity.Data;
using He_SheStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace He_SheStore.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Category> Categories { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Contactus> Contacts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    public DbSet<ProductSize> ProductSizes { get; set; }

    public DbSet<PaymentDetail> paymentDetails { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }
    public DbSet<AccountBalance> accountBalances { get; set; }

    public DbSet<testimonial> testimonials { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);      
    }
}
