using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechZone.Models.Models;


namespace TechZone.DataAccess.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext()
        {

        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCart> shoppingCarts { get; set; }
        public DbSet<Brand> brands { get; set; }
        public DbSet<OrderDetail> orderDetails { get; set; }
        public DbSet<OrderHeader> orderHeaders { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }




    }
}
