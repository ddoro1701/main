using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly string subscriptionKey;
        private readonly string endpoint;

        public ImageController(IConfiguration configuration)
        {
            subscriptionKey = configuration["AzureComputerVision:SubscriptionKey"];
            endpoint = configuration["AzureComputerVision:Endpoint"];
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image uploaded.");

            var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey))
            {
                Endpoint = endpoint
            };

            using (var stream = image.OpenReadStream())
            {
                var result = await client.RecognizePrintedTextInStreamAsync(true, stream);
                var text = string.Join(" ", result.Regions.SelectMany(r => r.Lines).SelectMany(l => l.Words).Select(w => w.Text));
                return Ok(new { text });
            }
        }
    }
}