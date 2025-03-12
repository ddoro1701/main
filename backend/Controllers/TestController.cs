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
                var emails = await _lecturers.Find(_ => true).Project(l => l.Email).ToListAsync();
                return Ok(emails);
            }
            catch (Exception ex)
            {
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

        // DELETE: api/lecturer/emails?email=someemail@example.com
        // In TestController.cs (LecturerController)
        [HttpDelete("emails")]
        public async Task<IActionResult> DeleteEmail([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { error = "Email is required." });
            }

            try
            {
                var result = await _lecturers.DeleteOneAsync(l => l.Email == email);
                if (result.DeletedCount == 0)
                {
                    return NotFound(new { error = $"No lecturer found with email {email}." });
                }
                return Ok(new { message = $"Email {email} deleted." }); // Return valid JSON
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        error = "An error occurred while deleting the lecturer email.",
                        details = ex.Message,
                    }
                );
            }
        }
    }
}
