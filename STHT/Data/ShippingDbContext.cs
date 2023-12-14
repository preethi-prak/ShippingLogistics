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

    public void CreateOrUpdateShipping(Shipping shippingModel)
    {
        var existingShipping =  ShippingDetails
            .FirstOrDefault(s => s.UserId == shippingModel.UserId && s.ProductId == shippingModel.ProductId);

        if (existingShipping == null)
        {
            ShippingDetails.Add(shippingModel);
            
        }
        else
        {
            // Update properties
            // ...
            ShippingDetails.Update(existingShipping);
        }
        SaveChanges();

    }
}