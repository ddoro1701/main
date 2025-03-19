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
            var email = await _matcher.FindLecturerEmailAsync(ocrText);
            if (email is null)
            {
                return NotFound("No matching lecturer email found.");
            }
            return Ok(new { email });
        }
    }
}