using BusinessObject.Payments;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace PRN232.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        /// <summary>
        /// Get a single active package by Id
        /// </summary>
        [HttpGet("{id}")]
    
        public async Task<IActionResult> GetById(int id)
        {
            var package = await _packageService.GetPackageByIdAsync(id);
            if (package == null)
                return NotFound($"Package with Id {id} not found or inactive.");

            return Ok(package);
        }

        /// <summary>
        /// Get all active packages (ordered by price asc)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var packages = await _packageService.GetAvailablePackagesAsync();
            return Ok(packages);
        }
    }
}
