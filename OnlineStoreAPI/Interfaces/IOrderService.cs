using OnlineStoreAPI.Models;

namespace OnlineStoreAPI.Interfaces;

public interface IOrderService
{
    public Order GetOrder(int id);
    public IEnumerable<Order> GetUserOrders(int id);
    public bool CreateOrder(int userId, int addressId, float totalPrice);
    public bool AddProductToOrder(int orderId, int productId, int quantity);
    public bool UpdateOrderStatus(int id, string status);
}