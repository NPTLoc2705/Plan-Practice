using BusinessObject.Dtos.paymentDto;
using BusinessObject.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IPackageService
    {
        Task<Package?> GetPackageByIdAsync(int packageId);
       Task<List<Package>> GetAvailablePackagesAsync();

        Task<PackageDetailDto> GetPackageDetailAsync(int packageId);
        Task<List<PackageListDto>> GetAllPackagesAsync(bool includeInactive = true);
        Task<(List<PackageListDto> packages, int totalCount)> GetPaginatedPackagesAsync(
            int pageNumber, int pageSize, string? searchTerm = null, bool? isActive = null);
        Task<PackageDetailDto> CreatePackageAsync(CreatePackageDto createDto);
        Task<PackageDetailDto> UpdatePackageAsync(int packageId, UpdatePackageDto updateDto);
        Task<bool> DeletePackageAsync(int packageId);
    }
}
