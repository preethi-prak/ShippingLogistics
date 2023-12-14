namespace STHT.Pages;


public class Shipping
{
    public int UserId { get; init; }
    public int ProductId { get; init; }
    public string? DeliveryOption { get; set; }
    
    public string? countryLocale { get; set; }
    
    public float ShippingCost { get; set; }
    
    public float OwnTransport { get; set; }
    public float BidPrice { get; set; }
    
    public float TotalPrice { get; set; }
    
}