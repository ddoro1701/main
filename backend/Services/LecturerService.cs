using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class LecturerService
    {
        private readonly IMongoCollection<Lecturer> _lecturers;

        public LecturerService(IConfiguration configuration)
        {
            string connectionString = configuration["COSMOS_CONNECTION_STRING"]
                ?? throw new Exception("COSMOS_CONNECTION_STRING is not set.");
            string databaseName = configuration["COSMOS_DATABASE_NAME"]
                ?? throw new Exception("COSMOS_DATABASE_NAME is not set.");

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _lecturers = database.GetCollection<Lecturer>("Lecturer");
        }

        public async Task<List<Lecturer>> GetAllLecturersAsync()
        {
            return await _lecturers.Find(_ => true).ToListAsync();
        }
    }
}