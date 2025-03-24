using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackageController : ControllerBase
    {
        private readonly PackageService _packageService;

        public PackageController(PackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmailToLecturer([FromBody] Package package)
        {
            if (package == null || string.IsNullOrWhiteSpace(package.LecturerEmail))
            {
                return BadRequest("Missing required package data.");
            }

            // Ensure CollectionDate only contains the date part (or defaults to current UTC date)
            if (package.CollectionDate.HasValue)
            {
                package.CollectionDate = package.CollectionDate.Value.Date;
            }
            else
            {
                package.CollectionDate = DateTime.UtcNow.Date;
            }

            // Set default status if not provided
            if (string.IsNullOrWhiteSpace(package.Status))
            {
                package.Status = "Received";
            }

            await _packageService.AddPackageAsync(package);
            return Ok(new { message = "Package record created successfully." });
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPackages()
        {
            var packages = await _packageService.GetAllPackagesAsync();
            return Ok(packages);
        }
        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Id))
            {
                return BadRequest(new { message = "Invalid package id." });
            }

            // Update the package status in the Packages collection.
            await _packageService.UpdatePackageStatusAsync(request.Id, request.Status);
            return Ok(new { message = "Status updated successfully." });
        }
        [HttpDelete("delete-collected")]
        public async Task<IActionResult> DeleteCollected([FromBody] DeleteCollectedRequest request)
        {
            if (request == null || request.Ids == null || request.Ids.Length == 0)
            {
                return BadRequest(new { message = "No IDs provided." });
            }

            await _packageService.DeleteCollectedAsync(request.Ids);
            return Ok(new { message = "Collected entries deleted." });
        }
    }

}