using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Services
{
    public class CosmosDbService
    {
        private Container _container;

        public CosmosDbService(CosmosClient dbClient, string databaseName, string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddItemAsync<T>(T item)
        {
            await _container.CreateItemAsync(item);
        }

        public async Task<List<T>> GetItemsAsync<T>(string queryString)
        {
            var query = _container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
            var results = new List<T>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }
        public async Task AddItemAsync<T>(T item, string containerName)
        {
            await _container.CreateItemAsync(item);
        }
    }
}