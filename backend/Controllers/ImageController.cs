using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;


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

            // Preprocess the image
            using (var inputStream = image.OpenReadStream())
            using (var outputStream = new MemoryStream())
            {
                // Load the image using ImageSharp
                var img = await Image.LoadAsync<Rgba32>(inputStream);

                // Preprocessing steps
                img.Mutate(x =>
                {
                    x.AutoOrient(); // Correct orientation
                    x.Contrast(1.2f); // Increase contrast
                    x.Brightness(1.1f); // Slightly increase brightness
                    x.Grayscale(); // Convert to grayscale
                    x.Crop(new Rectangle(0, 0, img.Width, img.Height)); // Crop (adjust as needed)
                });

                // Save the processed image to a memory stream
                await img.SaveAsJpegAsync(outputStream);
                outputStream.Seek(0, SeekOrigin.Begin);

                // Send the processed image to Azure OCR
                var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey))
                {
                    Endpoint = endpoint
                };

                var result = await client.ReadInStreamAsync(outputStream);
                var operationId = result.OperationLocation.Split('/').Last();

                // Poll for the result
                ReadOperationResult readResult;
                do
                {
                    await Task.Delay(1000);
                    readResult = await client.GetReadResultAsync(Guid.Parse(operationId));
                } while (readResult.Status == OperationStatusCodes.Running || readResult.Status == OperationStatusCodes.NotStarted);

                if (readResult.Status == OperationStatusCodes.Failed)
                    return BadRequest("Failed to process the image.");

                var text = string.Join(" ", readResult.AnalyzeResult.ReadResults
                    .SelectMany(r => r.Lines)
                    .Select(l => l.Text));

                return Ok(new { text });
            }
        }
    }
}