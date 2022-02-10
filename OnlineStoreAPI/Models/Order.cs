namespace OnlineStoreAPI.Models;

public class Order
{
    public int Id { get; set; }
    public User User { get; set; } = null!;
    public IEnumerable<OrderProduct> Products { get; set; } = null!;
    public Address Address { get; set; } = null!;
    public float TotalPrice { get; set; }
    public string Status { get; set; } = null!;
    public DateTime Timestamp { get; set; }
}