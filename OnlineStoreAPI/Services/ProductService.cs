using MySqlConnector;
using OnlineStoreAPI.Interfaces;
using OnlineStoreAPI.Models;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace OnlineStoreAPI.Services;

public class ProductService : IProductService
{
    public Product GetProduct(int id)
    {
        var product = new Product();

        using var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
        const string commandString = "select * from online_store.products where id = @id";
        var command = new MySqlCommand(commandString, connection);

        command.Parameters.AddWithValue("@id", id);
        
        connection.Open();

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            product.Id = (int) reader["id"];
            product.Name = (string) reader["name"];
            product.Description = (string) reader["description"];
            product.Price = (float) reader["price"];
            product.Stock = (int) reader["stock"];
            product.ImageUrl = (string) reader["image_url"];
        }

        return product;
    }

    public IEnumerable<Product> GetAllProducts()
    {
        var list = new List<Product>();

        using var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
        const string commandString = "select * from online_store.products";
        var command = new MySqlCommand(commandString, connection);

        connection.Open();

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Product
            {
                Id = (int) reader["id"],
                Name = (string) reader["name"],
                Description = (string) reader["description"],
                Price = (float) reader["price"],
                Stock = (int) reader["stock"],
                ImageUrl = (string) reader["image_url"]
            });
        }

        return list;
    }
}