using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LecturerController : ControllerBase
    {
        private readonly IMongoCollection<Lecturer> _lecturers;

        public LecturerController()
        {
            string connectionString =
                Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING")
                ?? throw new Exception("COSMOS_CONNECTION_STRING is not set.");
            string databaseName =
                Environment.GetEnvironmentVariable("COSMOS_DATABASE_NAME")
                ?? throw new Exception("COSMOS_DATABASE_NAME is not set.");

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _lecturers = database.GetCollection<Lecturer>("Lecturer");
        }

        // GET: api/lecturer/emails
        [HttpGet("emails")]
        public async Task<IActionResult> GetEmails()
        {
            try
            {
                // Retrieve just the Lecturer_Email field from each Lecturer document
                var emails = await _lecturers.Find(_ => true).Project(l => l.Email).ToListAsync();

                return Ok(emails);
            }
            catch (Exception ex)
            {
                // Return 500 with error message in case something goes wrong
                return StatusCode(
                    500,
                    new
                    {
                        error = "An error occurred while retrieving emails.",
                        details = ex.Message,
                    }
                );
            }
        }

        // POST: api/lecturer/emails
        [HttpPost("emails")]
        public async Task<IActionResult> CreateEmail([FromBody] Lecturer lecturer)
        {
            if (lecturer == null || string.IsNullOrEmpty(lecturer.Email))
            {
                return BadRequest("Lecturer with an Email is required.");
            }

            try
            {
                await _lecturers.InsertOneAsync(lecturer);
                return CreatedAtAction(nameof(GetEmails), new { id = lecturer.Id }, lecturer);
            }
            catch (Exception ex)
            {
                // Return 500 with error message in case something goes wrong
                return StatusCode(
                    500,
                    new
                    {
                        error = "An error occurred while creating the lecturer.",
                        details = ex.Message,
                    }
                );
            }
        }
    }
}
