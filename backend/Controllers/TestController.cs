using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        // You can remove the CosmosDbService dependency if you switch fully to MongoDB queries.
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Retrieve the connection string and database name from environment/config
                string connectionString =
                    Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING")
                    ?? throw new Exception("COSMOS_CONNECTION_STRING is not set.");

                string databaseName =
                    Environment.GetEnvironmentVariable("COSMOS_DATABASE_NAME")
                    ?? throw new Exception("COSMOS_DATABASE_NAME is not set.");

                // Create the MongoClient and get the database and collection
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(databaseName);
                var collection = database.GetCollection<dynamic>("Lecturer"); // Collection name "Lecturer"

                // Retrieve all documents from the collection
                var items = await collection.Find(_ => true).ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                // Log the exception as needed and return a 500 Internal Server Error
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
