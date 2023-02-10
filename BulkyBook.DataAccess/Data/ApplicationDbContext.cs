using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BulkyBook.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
        {
            public ApplicationDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer("Server=(LocalDB)\\MSSQLLocalDB;Database=Bulky;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True;");
                return new ApplicationDbContext(optionsBuilder.Options);
            }
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cover> Covers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Trolley> Trolleys { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
