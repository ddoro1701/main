using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication1.Services; // Hinzufügen
using WebApplication1.Models; // Hinzufügen

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackagesController : ControllerBase
    {
        private readonly CosmosDbService _cosmosDbService;

        public PackagesController(CosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePackage([FromBody] Package package)
        {
            await _cosmosDbService.AddItemAsync(package);
            return Ok();
        }

        // Weitere Methoden zum Abrufen, Aktualisieren und Löschen von Daten
    }
}