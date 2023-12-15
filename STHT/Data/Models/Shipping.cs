using System.ComponentModel.DataAnnotations;

namespace STHT.Data.Models;

public class Shipping
{
    public int Id { get; set; }
    [Required] public int UserId { get; init; }
    [Required] public int ProductId { get; init; }

    [MaxLength(50)] [Required] public string DeliveryOption { get; set; } = "OwnTransport";

    [MaxLength(2)] [Required] public string CountryLocale { get; set; } = "FR";

    [Required] public decimal ShippingCost { get; set; }

    public decimal OwnTransport { get; set; }

    [Required] public decimal BidPrice { get; set; }

    public decimal TotalPrice { get; set; }
}