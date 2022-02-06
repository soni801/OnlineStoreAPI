namespace OnlineStoreAPI.Models;

public class OrderProduct
{
    public int OrderId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
}