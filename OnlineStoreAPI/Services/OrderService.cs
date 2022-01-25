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
        const string commandString = "select * from online_store.orders, online_store.users, online_store.credentials, online_store.products, online_store.addresses, online_store.postal_places where user_id = users.id and users.username = credentials.username and product_id = products.id and address_id = addresses.id and addresses.postal_number = postal_places.postal_number and orders.id = @id";
        var command = new MySqlCommand(commandString, connection);

        command.Parameters.AddWithValue("@id", id);

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
            order.Product = new Product
            {
                Id = (int) reader["product_id"],
                Name = (string) reader["name"],
                Description = (string) reader["description"],
                Price = (float) reader["price"],
                Stock = (int) reader["stock"],
                ImageUrl = (string) reader["image_url"]
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

        return order;
    }
}