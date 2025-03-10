using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Threading.Tasks;
using WebApplication1.Services; // Hinzufügen

namespace WebApplication1.Models
{
    public class Lecturer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("Name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("Email")]
        public string Email { get; set; } = string.Empty;
    }

    public class PackageService
    {
        private readonly LecturerService _lecturerService;
        private readonly IMongoCollection<Package> _packages;

        public PackageService(LecturerService lecturerService, IMongoCollection<Package> packages)
        {
            _lecturerService = lecturerService;
            _packages = packages;
        }

        public async Task AddPackageAsync(Package package)
        {
            var lecturer = await _lecturerService.GetLecturerByNameAsync(package.RecipientName);

            if (lecturer != null)
            {
                package.LecturerEmail = lecturer.Email;
                package.Status = "Notified";
                await SendEmailNotification(lecturer.Email, package);
            }
            else
            {
                package.Status = "Unknown Lecturer";
            }

            await _packages.InsertOneAsync(package);
        }

        private async Task SendEmailNotification(string email, Package package)
        {
            Console.WriteLine($"📧 Sende E-Mail an: {email}");
            Console.WriteLine($"📦 Paket {package.TrackingNumber} ist zur Abholung bereit!");
            await Task.CompletedTask;
        }
    }
}