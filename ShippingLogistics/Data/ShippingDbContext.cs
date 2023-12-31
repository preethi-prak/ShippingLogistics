using Microsoft.EntityFrameworkCore;
using ShippingLogistics.Data.Models;
using ShippingLogistics.Pages;

namespace ShippingLogistics.Data;

public class ShippingDbContext : DbContext
{
    // for configuring connection string in the program.cs file such as the kind of database 
    public ShippingDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Shipping> ShippingDetails { get; set; }
    

    //Change the method name 
    public void CreateOrUpdateShippingForBidding(Shipping shippingModel)
    {
        var existingShipping = ShippingDetails
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
            if (shippingModel.DeliveryOption == "DeliveryToYard")
            {
                shippingModel.TotalPrice = shippingModel.ShippingCost + shippingModel.BidPrice;
            }
            else if (shippingModel.DeliveryOption == "OwnTransport")
            {
                shippingModel.TotalPrice = shippingModel.BidPrice;
            }

            existingShipping.TotalPrice = shippingModel.TotalPrice;

            ShippingDetails.Update(existingShipping);
        }

        SaveChanges();
    }

    public void CreateOrUpdateShipping(Shipping newShipping)
    {
        var existingShipping = ShippingDetails
            .FirstOrDefault(s => s.UserId == newShipping.UserId && s.ProductId == newShipping.ProductId);
        if (existingShipping == null)
        {
            //create new field
            ShippingDetails.Add(newShipping);
        }
        else
        {
            if (newShipping.DeliveryOption != existingShipping.DeliveryOption)
            {
                existingShipping.ShippingCost = newShipping.ShippingCost;
                existingShipping.OwnTransport = newShipping.OwnTransport;
                existingShipping.DeliveryOption = newShipping.DeliveryOption;
                existingShipping.CountryLocale = newShipping.CountryLocale;
                ShippingDetails.Update(existingShipping);
            }
        }

        SaveChanges();
    }
}