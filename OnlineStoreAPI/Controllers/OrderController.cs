using Microsoft.AspNetCore.Mvc;
using OnlineStoreAPI.Interfaces;
using OnlineStoreAPI.Models;
using OnlineStoreAPI.Models.Requests;

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
    public int CreateOrder([FromBody] CreateOrderRequest payload)
    {
        return _orderService.CreateOrder(payload.Token, payload.TotalPrice, payload.AddressName, payload.AddressLine, payload.PostalNumber, payload.Country);
    }

    [HttpPost("link")]
    public bool AddProductToOrder(int orderId, int productId, int quantity)
    {
        return _orderService.AddProductToOrder(orderId, productId, quantity);
    }

    [HttpPost("update")]
    public bool UpdateOrderStatus(int id, string status)
    {
        return _orderService.UpdateOrderStatus(id, status);
    }
}
