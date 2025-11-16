using BusinessObject;
using BusinessObject.Dtos.Admin;
using BusinessObject.Dtos.paymentDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interface;
using Service.Method;

namespace PRN232.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IPaymentService _paymentService;
        private readonly IPackageService _packageService;

        public AdminController(IAdminService adminService, IPackageService packageService, IPaymentService paymentService)
        {
            _adminService = adminService;
            _paymentService = paymentService;
            _packageService = packageService;
        }

        // Helper method to ensure DateTime is UTC
        private static DateTime EnsureUtc(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;

            if (dateTime.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

            return dateTime.ToUniversalTime();
        }

        // GET: api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] string? role = null)
        {
            try
            {
                UserRole? roleFilter = null;
                if (!string.IsNullOrWhiteSpace(role) && Enum.TryParse<UserRole>(role, true, out var parsedRole))
                {
                    roleFilter = parsedRole;
                }

                var users = await _adminService.GetAllUsersAsync(roleFilter);
                return Ok(new
                {
                    message = "Users retrieved successfully",
                    data = users,
                    count = users.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/admin/students
        [HttpGet("students")]
        public async Task<IActionResult> GetAllStudents()
        {
            try
            {
                var students = await _adminService.GetAllStudentsAsync();
                return Ok(new
                {
                    message = "Students retrieved successfully",
                    data = students,
                    count = students.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/admin/teachers
        [HttpGet("teachers")]
        public async Task<IActionResult> GetAllTeachers()
        {
            try
            {
                var teachers = await _adminService.GetAllTeachersAsync();
                return Ok(new
                {
                    message = "Teachers retrieved successfully",
                    data = teachers,
                    count = teachers.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/admin/users/paginated
        [HttpGet("users/paginated")]
        public async Task<IActionResult> GetPaginatedUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? role = null)
        {
            try
            {
                UserRole? roleFilter = null;
                if (!string.IsNullOrWhiteSpace(role) && Enum.TryParse<UserRole>(role, true, out var parsedRole))
                {
                    roleFilter = parsedRole;
                }

                var (users, totalCount) = await _adminService.GetPaginatedUsersAsync(
                    pageNumber, pageSize, searchTerm, roleFilter);

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                return Ok(new
                {
                    message = "Users retrieved successfully",
                    data = new
                    {
                        items = users,
                        totalCount = totalCount,
                        pageNumber = pageNumber,
                        pageSize = pageSize,
                        totalPages = totalPages,
                        hasNextPage = pageNumber < totalPages,
                        hasPreviousPage = pageNumber > 1
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/admin/users/5
        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var user = await _adminService.GetUserByIdAsync(userId);
                return Ok(new
                {
                    message = "User retrieved successfully",
                    data = user
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/admin/users/5
        [HttpPut("users/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new
                    {
                        message = "Validation failed",
                        errors = errors
                    });
                }

                var updatedUser = await _adminService.UpdateUserAsync(userId, updateDto);

                return Ok(new
                {
                    message = "User updated successfully",
                    data = updatedUser
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/admin/users/5/ban
        [HttpPost("users/{userId}/ban")]
        public async Task<IActionResult> BanUser(int userId)
        {
            try
            {
                var message = await _adminService.BanUserAsync(userId);
                return Ok(new { message = message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/admin/users/5/unban
        [HttpPost("users/{userId}/unban")]
        public async Task<IActionResult> UnbanUser(int userId)
        {
            try
            {
                var message = await _adminService.UnbanUserAsync(userId);
                return Ok(new { message = message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/admin/transactions
        [HttpGet("transactions")]
        public async Task<IActionResult> GetAllPaidTransactions(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                // Ensure UTC if dates provided
                DateTime? utcStartDate = startDate.HasValue ? EnsureUtc(startDate.Value) : null;
                DateTime? utcEndDate = endDate.HasValue ? EnsureUtc(endDate.Value) : null;

                var transactions = await _paymentService.GetAllPaidTransactionsAsync(utcStartDate, utcEndDate);

                return Ok(new
                {
                    message = "Paid transactions retrieved successfully",
                    data = transactions,
                    count = transactions.Count,
                    filters = new
                    {
                        startDate = utcStartDate?.ToString("yyyy-MM-dd"),
                        endDate = utcEndDate?.ToString("yyyy-MM-dd")
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/admin/transactions/paginated
        [HttpGet("transactions/paginated")]
        public async Task<IActionResult> GetPaginatedPaidTransactions(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                // Ensure UTC if dates provided
                DateTime? utcStartDate = startDate.HasValue ? EnsureUtc(startDate.Value) : null;
                DateTime? utcEndDate = endDate.HasValue ? EnsureUtc(endDate.Value) : null;

                var (transactions, totalCount) = await _paymentService.GetPaginatedPaidTransactionsAsync(
                    pageNumber, pageSize, utcStartDate, utcEndDate);

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                return Ok(new
                {
                    message = "Paginated transactions retrieved successfully",
                    data = new
                    {
                        items = transactions,
                        currentPage = pageNumber,
                        pageSize = pageSize,
                        totalCount = totalCount,
                        totalPages = totalPages,
                        hasNextPage = pageNumber < totalPages,
                        hasPreviousPage = pageNumber > 1
                    },
                    filters = new
                    {
                        startDate = utcStartDate?.ToString("yyyy-MM-dd"),
                        endDate = utcEndDate?.ToString("yyyy-MM-dd")
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/admin/revenue
        [HttpGet("revenue")]
        public async Task<IActionResult> GetTotalRevenue(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                // Ensure UTC if dates provided
                DateTime? utcStartDate = startDate.HasValue ? EnsureUtc(startDate.Value) : null;
                DateTime? utcEndDate = endDate.HasValue ? EnsureUtc(endDate.Value) : null;

                var revenue = await _paymentService.GetTotalRevenueAsync(utcStartDate, utcEndDate);

                return Ok(new
                {
                    message = "Revenue statistics retrieved successfully",
                    data = revenue
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/admin/revenue/today
        [HttpGet("revenue/today")]
        public async Task<IActionResult> GetTodayRevenue()
        {
            try
            {
                var now = DateTime.UtcNow;
                var today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
                var tomorrow = today.AddDays(1);

                var revenue = await _paymentService.GetTotalRevenueAsync(today, tomorrow);

                return Ok(new
                {
                    message = "Today's revenue retrieved successfully",
                    data = revenue
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/admin/revenue/month
        [HttpGet("revenue/month")]
        public async Task<IActionResult> GetMonthRevenue()
        {
            try
            {
                var now = DateTime.UtcNow;
                var firstDayOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

                var revenue = await _paymentService.GetTotalRevenueAsync(firstDayOfMonth, firstDayOfNextMonth);

                return Ok(new
                {
                    message = "This month's revenue retrieved successfully",
                    data = revenue
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/admin/revenue/year
        [HttpGet("revenue/year")]
        public async Task<IActionResult> GetYearRevenue()
        {
            try
            {
                var now = DateTime.UtcNow;
                var firstDayOfYear = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var firstDayOfNextYear = firstDayOfYear.AddYears(1);

                var revenue = await _paymentService.GetTotalRevenueAsync(firstDayOfYear, firstDayOfNextYear);

                return Ok(new
                {
                    message = "This year's revenue retrieved successfully",
                    data = revenue
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/admin/revenue/last30days
        [HttpGet("revenue/last30days")]
        public async Task<IActionResult> GetLast30DaysRevenue()
        {
            try
            {
                var endDate = DateTime.UtcNow;
                var startDate = endDate.AddDays(-30);

                var revenue = await _paymentService.GetTotalRevenueAsync(startDate, endDate);

                return Ok(new
                {
                    message = "Last 30 days revenue retrieved successfully",
                    data = revenue
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        /// <summary>
        /// Get all packages with optional filtering
        /// </summary>
        [HttpGet("packages")]
        public async Task<IActionResult> GetAllPackages([FromQuery] bool? includeInactive = true)
        {
            try
            {
                var packages = await _packageService.GetAllPackagesAsync(includeInactive ?? true);
                return Ok(new
                {
                    message = "Packages retrieved successfully",
                    data = packages,
                    count = packages.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get paginated packages with search and filter
        /// </summary>
        [HttpGet("packages/paginated")]
        public async Task<IActionResult> GetPaginatedPackages(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                var (packages, totalCount) = await _packageService.GetPaginatedPackagesAsync(
                    pageNumber, pageSize, searchTerm, isActive);

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                return Ok(new
                {
                    message = "Packages retrieved successfully",
                    data = new
                    {
                        items = packages,
                        totalCount = totalCount,
                        pageNumber = pageNumber,
                        pageSize = pageSize,
                        totalPages = totalPages,
                        hasNextPage = pageNumber < totalPages,
                        hasPreviousPage = pageNumber > 1
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get package details by ID including statistics
        /// </summary>
        [HttpGet("packages/{packageId}")]
        public async Task<IActionResult> GetPackageDetail(int packageId)
        {
            try
            {
                var package = await _packageService.GetPackageDetailAsync(packageId);
                return Ok(new
                {
                    message = "Package details retrieved successfully",
                    data = package
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Create a new package
        /// </summary>
        [HttpPost("packages")]
        public async Task<IActionResult> CreatePackage([FromBody] CreatePackageDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var package = await _packageService.CreatePackageAsync(createDto);

                return CreatedAtAction(
                    nameof(GetPackageDetail),
                    new { packageId = package.Id },
                    new
                    {
                        message = "Package created successfully",
                        data = package
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing package
        /// </summary>
        [HttpPut("packages/{packageId}")]
        public async Task<IActionResult> UpdatePackage(int packageId, [FromBody] UpdatePackageDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var package = await _packageService.UpdatePackageAsync(packageId, updateDto);

                return Ok(new
                {
                    message = "Package updated successfully",
                    data = package
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a package (soft delete if has payments, hard delete otherwise)
        /// </summary>
        [HttpDelete("packages/{packageId}")]
        public async Task<IActionResult> DeletePackage(int packageId)
        {
            try
            {
                var result = await _packageService.DeletePackageAsync(packageId);

                if (result)
                {
                    return Ok(new { message = "Package deleted successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Failed to delete package" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}