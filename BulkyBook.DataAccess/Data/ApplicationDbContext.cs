﻿using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BulkyBook.DataAccess
{
    public class ApplicationDbContext : DbContext
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
    }
}
