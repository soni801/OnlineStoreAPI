namespace OnlineStoreAPI.Models.Requests;

public class CreateOrderRequest
{
    public string Token { get; set; } = null!;
    public float TotalPrice { get; set; }
    public string AddressName { get; set; } = null!;
    public string AddressLine { get; set; } = null!;
    public string PostalNumber { get; set; } = null!;
    public string Country { get; set; } = null!;
}
