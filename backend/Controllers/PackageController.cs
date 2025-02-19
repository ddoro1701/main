using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using WebApplication1.Backend.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackageController : ControllerBase
    {
        private readonly ILogger<PackageController> _logger;

        public PackageController(ILogger<PackageController> logger)
        {
            _logger = logger;
        }

        // GET: api/package
        [HttpGet]
        public IActionResult GetPackages()
        {
            try
            {
                // Logic to get packages
                var packages = new List<Package>();
                return Ok(packages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting packages.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/package
        [HttpPost]
        public IActionResult CreatePackage([FromBody] Package package)
        {
            try
            {
                if (package == null)
                {
                    return BadRequest("Package is null.");
                }

                // Logic to create a package
                return CreatedAtAction(nameof(GetPackages), new { id = package.Id }, package);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating package.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}