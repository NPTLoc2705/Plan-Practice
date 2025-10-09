using BusinessObject;
using BusinessObject.Dtos.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interface;

namespace PRN232.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Only Admin can access these endpoints
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
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

        // GET: api/admin/users/paginated?pageNumber=1&pageSize=10&searchTerm=john&role=Student
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

                var response = new PaginatedResponse<UserListDto>
                {
                    Data = users,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return Ok(new
                {
                    message = "Users retrieved successfully",
                    pagination = response
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
                    return BadRequest(ModelState);
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

      
    }
}