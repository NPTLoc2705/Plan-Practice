using BusinessObject.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IPackageRepo
    {
        Task<Package?> FindPackageById(int packageId, bool includeInactive = false);
        Task<List<Package>> GetAvailablePackage();
        Task<List<Package>> GetAllPackages(bool includeInactive = true);
        Task<(List<Package> packages, int totalCount)> GetPaginatedPackages(
            int pageNumber, int pageSize, string? searchTerm = null, bool? isActive = null);
        Task<Package> CreatePackage(Package package);
        Task<Package> UpdatePackage(Package package);
        Task<bool> DeletePackage(int packageId);
        Task<bool> PackageExistsByName(string name, int? excludeId = null);
        Task<(int totalPurchases, int totalRevenue)> GetPackageStatistics(int packageId);
    }
}

