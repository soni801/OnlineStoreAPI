using MySqlConnector;
using OnlineStoreAPI.Interfaces;
using OnlineStoreAPI.Models;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace OnlineStoreAPI.Services;

public class OrderService : IOrderService
{
    public Order GetOrder(int id)
    {
        var order = new Order();
        
        using var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
        const string commandString = "select * from online_store.orders, online_store.users, online_store.credentials, online_store.addresses, online_store.postal_places where user_id = users.id and users.username = credentials.username and address_id = addresses.id and addresses.postal_number = postal_places.postal_number and orders.id = @id";
        var command = new MySqlCommand(commandString, connection);
        command.Parameters.AddWithValue("@id", id);

        const string productsCommandString = "select products.*, quantity from online_store.orders, online_store.products, online_store.orders_products where orders.id = orders_products.order_id and products.id = orders_products.product_id and orders.id = @id";
        var productsCommand = new MySqlCommand(productsCommandString, connection);
        productsCommand.Parameters.AddWithValue("@id", id);

        connection.Open();
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            order.Id = (int) reader[0];
            order.User = new User
            {
                Id = (int) reader["user_id"],
                FirstName = (string) reader["first_name"],
                LastName = (string) reader["last_name"],
                Credentials = new Credentials
                {
                    Username = (string) reader["username"],
                    Passphrase = (string) reader["passphrase"],
                    Token = (string) reader["token"],
                    AccessLevel = (int) reader["access_level"]
                },
                Email = (string) reader["email"],
                PhoneNumber = (int) reader["phone_number"]
            };
            order.Address = new Address
            {
                Id = (int) reader["address_id"],
                AddressName = (string) reader["address_name"],
                AddressLine = (string) reader["address_line"],
                PostalNumber = new PostalNumber
                {
                    Number = (int) reader["postal_number"],
                    Place = (string) reader["postal_place"]
                },
                Country = (string) reader["country"]
            };
            order.TotalPrice = (float) reader["total_price"];
            order.Timestamp = (DateTime) reader["timestamp"];
        }
        reader.Close(); // Close the active reader to use new reader

        // Get the products in the relevant order
        var list = new List<OrderProduct>();
        using var productsReader = productsCommand.ExecuteReader();
        while (productsReader.Read())
        {
            list.Add(new OrderProduct
            {
                OrderId = order.Id,
                Product = new Product
                {
                    Id = (int) productsReader["id"],
                    Name = (string) productsReader["name"],
                    Description = (string) productsReader["description"],
                    Price = (float) productsReader["price"],
                    Stock = (int) productsReader["stock"],
                    ImageUrl = (string) productsReader["image_url"]
                },
                Quantity = (int) productsReader["quantity"]
            });
        }

        order.Products = list;
        return order;
    }

    public IEnumerable<Order> GetUserOrders(int id)
    {
        var products = new List<OrderProduct>();
        var orders = new List<Order>();
        
        using var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
        
        const string productsString = "select products.*, quantity, order_id from online_store.orders, online_store.products, online_store.orders_products where orders.id = orders_products.order_id and products.id = orders_products.product_id and user_id = @id";
        var productsCommand = new MySqlCommand(productsString, connection);
        productsCommand.Parameters.AddWithValue("@id", id);

        const string ordersString = "select * from online_store.orders, online_store.users, online_store.credentials, online_store.addresses, online_store.postal_places where user_id = users.id and users.username = credentials.username and address_id = addresses.id and addresses.postal_number = postal_places.postal_number and user_id = @id";
        var ordersCommand = new MySqlCommand(ordersString, connection);
        ordersCommand.Parameters.AddWithValue("@id", id);
        
        connection.Open();

        using var productsReader = productsCommand.ExecuteReader();
        while (productsReader.Read())
        {
            products.Add(new OrderProduct
            {
                OrderId = (int) productsReader["order_id"],
                Product = new Product
                {
                    Id = (int) productsReader["id"],
                    Name = (string) productsReader["name"],
                    Description = (string) productsReader["description"],
                    Price = (float) productsReader["price"],
                    Stock = (int) productsReader["stock"],
                    ImageUrl = (string) productsReader["image_url"]
                },
                Quantity = (int) productsReader["quantity"]
            });
        }
        
        productsReader.Close();
        using var ordersReader = ordersCommand.ExecuteReader();
        while (ordersReader.Read())
        {
            var orderId = (int) ordersReader[0];
            orders.Add(new Order
            {
                Id = orderId,
                User = new User
                {
                    Id = (int) ordersReader["user_id"],
                    FirstName = (string) ordersReader["first_name"],
                    LastName = (string) ordersReader["last_name"],
                    Credentials = new Credentials
                    {
                        Username = (string) ordersReader["username"],
                        Passphrase = (string) ordersReader["passphrase"],
                        Token = (string) ordersReader["token"],
                        AccessLevel = (int) ordersReader["access_level"]
                    },
                    Email = (string) ordersReader["email"],
                    PhoneNumber = (int) ordersReader["phone_number"]
                },
                Products = products.Where(p => p.OrderId == orderId),
                Address = new Address
                {
                    Id = (int) ordersReader["address_id"],
                    AddressName = (string) ordersReader["address_name"],
                    AddressLine = (string) ordersReader["address_line"],
                    PostalNumber = new PostalNumber
                    {
                        Number = (int) ordersReader["postal_number"],
                        Place = (string) ordersReader["postal_place"]
                    },
                    Country = (string) ordersReader["country"]
                },
                TotalPrice = (float) ordersReader["total_price"],
                Timestamp = (DateTime) ordersReader["timestamp"]
            });
        }

        return orders;
    }

    public bool CreateOrder(int userId, int addressId, float totalPrice)
    {
        using var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
        const string commandString = "insert into online_store.orders (user_id, address_id, total_price) values (@userId, @addressId, @totalPrice)";
        var command = new MySqlCommand(commandString, connection);

        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@addressId", addressId);
        command.Parameters.AddWithValue("@totalPrice", totalPrice);

        try
        {
            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        
        return true;
    }

    public bool AddProductToOrder(int orderId, int productId, int quantity)
    {
        using var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
        const string commandString = "insert into online_store.orders_products (order_id, product_id, quantity) values (@orderId, @productId, @quantity)";
        var command = new MySqlCommand(commandString, connection);

        command.Parameters.AddWithValue("@orderId", orderId);
        command.Parameters.AddWithValue("@productId", productId);
        command.Parameters.AddWithValue("@quantity", quantity);

        try
        {
            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        
        return true;
    }
}