using OnlineStoreAPI.Models;

namespace OnlineStoreAPI.Interfaces;

public interface IOrderService
{
    public Order GetOrder(int id);
    public IEnumerable<Order> GetUserOrders(int id);
    public bool CreateOrder(string token, float totalPrice, string addressName, string addressLine, string postalNumber, string country);
    public bool AddProductToOrder(int orderId, int productId, int quantity);
    public bool UpdateOrderStatus(int id, string status);
}
