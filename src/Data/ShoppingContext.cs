

using Microsoft.EntityFrameworkCore;
using Owasp2021Top10.Models;

public class ShoppingContext : DbContext
{
    public ShoppingContext(DbContextOptions<ShoppingContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
}