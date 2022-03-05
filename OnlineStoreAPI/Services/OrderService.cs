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
                    Number = (string) reader["postal_number"],
                    Place = (string) reader["postal_place"]
                },
                Country = (string) reader["country"]
            };
            order.TotalPrice = (float) reader["total_price"];
            order.Status = (string) reader["status"];
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
                        Number = (string) ordersReader["postal_number"],
                        Place = (string) ordersReader["postal_place"]
                    },
                    Country = (string) ordersReader["country"]
                },
                TotalPrice = (float) ordersReader["total_price"],
                Status = (string) ordersReader["status"],
                Timestamp = (DateTime) ordersReader["timestamp"]
            });
        }

        return orders;
    }

    public bool CreateOrder(string token, float totalPrice, string addressName, string addressLine, string postalNumber, string country)
    {
        // Declare fields
        int? addressId = null;
        int? userId = null;
        
        // Create a connection
        using var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);

        // Command for checking if the address exists
        const string checkAddressString = "select id from online_store.addresses where address_name = @addressName and address_line = @addressLine and postal_number = @postalNumber and country = @country";
        var checkAddressCommand = new MySqlCommand(checkAddressString, connection);
        checkAddressCommand.Parameters.AddWithValue("@addressName", addressName);
        checkAddressCommand.Parameters.AddWithValue("@addressLine", addressLine);
        checkAddressCommand.Parameters.AddWithValue("@postalNumber", postalNumber);
        checkAddressCommand.Parameters.AddWithValue("@country", country);

        // Command for creating an address
        const string createAddressString = "insert into online_store.addresses (address_name, address_line, postal_number, country) values (@addressName, @addressLine, @postalNumber, @country)";
        var createAddressCommand = new MySqlCommand(createAddressString, connection);
        createAddressCommand.Parameters.AddWithValue("@addressName", addressName);
        createAddressCommand.Parameters.AddWithValue("@addressLine", addressLine);
        createAddressCommand.Parameters.AddWithValue("@postalNumber", postalNumber);
        createAddressCommand.Parameters.AddWithValue("@country", country);
        
        // Command for getting the user ID
        const string checkUserString = "select id from online_store.users, online_store.credentials where users.username = credentials.username and token = @token";
        var checkUserCommand = new MySqlCommand(checkUserString, connection);
        checkUserCommand.Parameters.AddWithValue("@token", token);

        // Command for creating the order
        const string createOrderString = "insert into online_store.orders (user_id, address_id, total_price) values (@userId, @addressId, @totalPrice)";
        var createOrderCommand = new MySqlCommand(createOrderString, connection);
        createOrderCommand.Parameters.AddWithValue("@totalPrice", totalPrice);

        try
        {
            // Open the connection
            connection.Open();
            
            // Check if the address exists
            using var addressReader = checkAddressCommand.ExecuteReader();
            while (addressReader.Read()) addressId = (int) addressReader[0];
            addressReader.Close();

            // If the address doesn't exist, create it
            if (addressId == null)
            {
                createAddressCommand.ExecuteNonQuery();

                // Get the ID of the newly created address
                using var addressConfirmReader = checkAddressCommand.ExecuteReader();
                while (addressConfirmReader.Read()) addressId = (int) addressConfirmReader[0];
                addressConfirmReader.Close();
            }

            // Get the ID of the user
            using var userReader = checkUserCommand.ExecuteReader();
            while (userReader.Read()) userId = (int) userReader[0];
            userReader.Close();

            // If the ID has not been acquired (likely means invalid token), exit and return false
            if (userId == null) return false;

            // Add the acquired parameters to createOrderCommand
            createOrderCommand.Parameters.AddWithValue("@userId", userId);
            createOrderCommand.Parameters.AddWithValue("@addressId", addressId);

            // Create the order
            createOrderCommand.ExecuteNonQuery();
            connection.Close();
        }
        catch (Exception e)
        {
            // Print any runtime errors
            Console.WriteLine(e);
            return false;
        }

        // Return true when execution succeeded
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

    public bool UpdateOrderStatus(int id, string status)
    {
        using var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
        const string commandString = "update online_store.orders set status = @status where id = @id";
        var command = new MySqlCommand(commandString, connection);
        
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@status", status);

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