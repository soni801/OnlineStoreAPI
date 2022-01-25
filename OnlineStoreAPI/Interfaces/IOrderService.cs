using OnlineStoreAPI.Models;

namespace OnlineStoreAPI.Interfaces;

public interface IOrderService
{
    public Order GetOrder(int id);
}