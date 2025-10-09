using BusinessObject;
using BusinessObject.Dtos.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IAdminService
    {
        Task<List<UserListDto>> GetAllUsersAsync(UserRole? roleFilter = null);
        Task<List<UserListDto>> GetAllStudentsAsync();
        Task<List<UserListDto>> GetAllTeachersAsync();
        Task<(List<UserListDto> users, int totalCount)> GetPaginatedUsersAsync(int pageNumber, int pageSize, string? searchTerm = null, UserRole? roleFilter = null);
        Task<UserDetailDto> GetUserByIdAsync(int userId);
        Task<UserDetailDto> UpdateUserAsync(int userId, UpdateUserDto updateDto);
        Task<string> BanUserAsync(int userId);
        Task<string> UnbanUserAsync(int userId);
    }
}
