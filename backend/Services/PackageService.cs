using MongoDB.Driver;
using WebApplication1.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Services
{
    public class PackageService
    {
        private readonly IMongoCollection<Package> _packages;

        public PackageService(IConfiguration configuration)
        {
            string connectionString = configuration["COSMOS_CONNECTION_STRING"]
                ?? throw new Exception("COSMOS_CONNECTION_STRING is not set.");
            string databaseName = configuration["COSMOS_DATABASE_NAME"]
                ?? throw new Exception("COSMOS_DATABASE_NAME is not set.");

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _packages = database.GetCollection<Package>("Packages");
        }

        public async Task AddPackageAsync(Package package)
        {
            await _packages.InsertOneAsync(package);
        }

        public async Task<List<Package>> GetAllPackagesAsync()
        {
            return await _packages.Find(_ => true).ToListAsync();
        }

        public async Task UpdatePackageStatusAsync(string id, string status)
        {
            var filter = Builders<Package>.Filter.Eq(p => p.Id, id);
            var update = Builders<Package>.Update.Set(p => p.Status, status);
            await _packages.UpdateOneAsync(filter, update);
        }
        public async Task DeleteCollectedAsync(string[] ids)
        {
            var filter = Builders<Package>.Filter.In(p => p.Id, ids);
            await _packages.DeleteManyAsync(filter);
        }
    }
}