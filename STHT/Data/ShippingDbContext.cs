using Microsoft.EntityFrameworkCore;
using STHT.Data.Models;
using STHT.Pages;

namespace STHT.Data;

public class ShippingDbContext : DbContext
{
    // for configuring connection string in the program.cs file such as the kind of database 
    public ShippingDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Shipping> ShippingDetails { get; set; }
    
}