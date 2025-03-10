using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class LecturerService
    {
        // Beispielmethode zum Abrufen eines Dozenten anhand des Namens
        public async Task<Lecturer> GetLecturerByNameAsync(string name)
        {
            // Implementieren Sie die Logik zum Abrufen des Dozenten aus der Datenbank
            // Dies ist nur ein Beispiel, passen Sie es an Ihre Anforderungen an
            return await Task.FromResult(new Lecturer { Name = name, Email = "example@example.com" });
        }
    }
}