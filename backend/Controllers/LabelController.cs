using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelController : ControllerBase
    {
        private readonly LecturerMatcher _matcher;

        public LabelController(LecturerMatcher matcher)
        {
            _matcher = matcher;
        }

        [HttpPost("find-email")]
        public async Task<IActionResult> FindEmail([FromBody] string ocrText)
        {
            if (string.IsNullOrWhiteSpace(ocrText))
            {
                return BadRequest("OCR text is empty. Please upload an image to generate OCR text.");
            }

            // Process the OCR text and use LecturerMatcher to determine the email.
            var email = await _matcher.FindLecturerEmailAsync(ocrText);
            return email == null
                   ? NotFound("Kein passendes Lecturer-Email gefunden.")
                   : Ok(new { email });
        }
    }
}