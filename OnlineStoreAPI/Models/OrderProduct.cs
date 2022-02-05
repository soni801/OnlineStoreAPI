namespace OnlineStoreAPI.Models;

public class OrderProduct
{
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
}