using BusinessObject.Payments;
using DAL;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Method
{
    public class PackageRepo : IPackageRepo
    {
        private readonly PackageDAO _packageDAO;
        public PackageRepo(PackageDAO packageDAO)
        {
            _packageDAO = packageDAO;
        }
        public async Task<Package?> FindPackageById(int packageId, bool includeInactive = false)
        {
            return await _packageDAO.FindPackageById(packageId, includeInactive);
        }

        public async Task<List<Package>> GetAvailablePackage()
        {
            return await _packageDAO.GetAllActivePackages();
        }

        public async Task<List<Package>> GetAllPackages(bool includeInactive = true)
        {
            return await _packageDAO.GetAllPackages(includeInactive);
        }

        public async Task<(List<Package> packages, int totalCount)> GetPaginatedPackages(
            int pageNumber, int pageSize, string? searchTerm = null, bool? isActive = null)
        {
            return await _packageDAO.GetPaginatedPackages(pageNumber, pageSize, searchTerm, isActive);
        }

        public async Task<Package> CreatePackage(Package package)
        {
            return await _packageDAO.CreatePackage(package);
        }

        public async Task<Package> UpdatePackage(Package package)
        {
            return await _packageDAO.UpdatePackage(package);
        }

        public async Task<bool> DeletePackage(int packageId)
        {
            return await _packageDAO.DeletePackage(packageId);
        }

        public async Task<bool> PackageExistsByName(string name, int? excludeId = null)
        {
            return await _packageDAO.PackageExistsByName(name, excludeId);
        }

        public async Task<(int totalPurchases, int totalRevenue)> GetPackageStatistics(int packageId)
        {
            return await _packageDAO.GetPackageStatistics(packageId);
        }
    }
}
