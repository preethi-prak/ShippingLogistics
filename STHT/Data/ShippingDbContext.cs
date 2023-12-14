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
            //create new field
            ShippingDetails.Add(shippingModel);
        }
        else
        {
            // Update only specific fields
            existingShipping.ShippingCost = shippingModel.ShippingCost;
            existingShipping.OwnTransport = shippingModel.OwnTransport;
            existingShipping.DeliveryOption = shippingModel.DeliveryOption;
            existingShipping.CountryLocale = shippingModel.CountryLocale;
            existingShipping.BidPrice = shippingModel.BidPrice;
            if (shippingModel.DeliveryOption =="DeliveryToYard")
            {
                shippingModel.TotalPrice = shippingModel.ShippingCost + shippingModel.BidPrice;
                
            }
            else if(shippingModel.DeliveryOption =="OwnTransport")
            {
                shippingModel.TotalPrice =  shippingModel.BidPrice;
            }
            existingShipping.TotalPrice = shippingModel.TotalPrice;
            
            ShippingDetails.Update(existingShipping);
        }
        SaveChanges();

    }
}