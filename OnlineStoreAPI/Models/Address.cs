namespace OnlineStoreAPI.Models;

public class Address
{
    public int Id { get; set; }
    public string AddressName { get; set; } = null!;
    public string AddressLine { get; set; } = null!;
    public PostalNumber PostalNumber { get; set; } = null!;
    public string Country { get; set; } = null!;
}