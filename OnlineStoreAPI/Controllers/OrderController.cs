using Microsoft.AspNetCore.Mvc;
using OnlineStoreAPI.Interfaces;
using OnlineStoreAPI.Models;

namespace OnlineStoreAPI.Controllers;

[ApiController]
[Route("orders")]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public Order GetOrder(int id)
    {
        return _orderService.GetOrder(id);
    }

    [HttpGet("user")]
    public IEnumerable<Order> GetUserOrders(int id)
    {
        return _orderService.GetUserOrders(id);
    }

    [HttpPost("new")]
    public bool CreateOrder(int userId, int addressId, float totalPrice)
    {
        return _orderService.CreateOrder(userId, addressId, totalPrice);
    }

    [HttpPost("link")]
    public bool AddProductToOrder(int orderId, int productId, int quantity)
    {
        return _orderService.AddProductToOrder(orderId, productId, quantity);
    }
}