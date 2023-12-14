using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STHT.Data.Models;


public class Shipping
{
    
    public int Id { get; set; }
    [Required]
    public int UserId { get; init; }
    [Required]
    public int ProductId { get; init; }
    [MaxLength(50)]
    [Required]
    public string? DeliveryOption { get; set; }
    [MaxLength(2)]
    public string? CountryLocale { get; set; }
    [Required]
    public decimal ShippingCost { get; set; }
    
    public decimal OwnTransport { get; set; }
    public decimal BidPrice { get; set; }
    
    public decimal TotalPrice { get; set; }
    
}