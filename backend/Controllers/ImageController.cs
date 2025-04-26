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
        private readonly bool isStudentAccount;

        public ImageController(IConfiguration configuration)
        {
            subscriptionKey = configuration["AzureComputerVision:SubscriptionKey"];
            endpoint = configuration["AzureComputerVision:Endpoint"];
            isStudentAccount = bool.Parse(configuration["AzureAccount:IsStudentAccount"]);
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

                // Check if the image is too large and resize if necessary (only for student accounts)
                const int maxWidth = 1024;
                const int maxHeight = 768;

                if (isStudentAccount && (img.Width > maxWidth || img.Height > maxHeight))
                {
                    img.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(maxWidth, maxHeight)
                    }));
                    Console.WriteLine($"Image resized to: {img.Width}x{img.Height}");
                }

                // Additional preprocessing steps
                img.Mutate(x =>
                {
                    x.AutoOrient(); // Correct orientation
                    x.Contrast(1.2f); // Increase contrast
                    x.Brightness(1.1f); // Slightly increase brightness
                    x.Grayscale(); // Convert to grayscale
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