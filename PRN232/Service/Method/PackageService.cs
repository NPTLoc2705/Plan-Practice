using BusinessObject.Payments;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Method
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepo _packageRepo;

        public PackageService(IPackageRepo packageRepo)
        {
            _packageRepo = packageRepo;
        }

        public async Task<Package?> GetPackageByIdAsync(int packageId)
        {
            if (packageId <= 0)
                throw new ArgumentException("Package Id must be greater than zero.", nameof(packageId));

            var package = await _packageRepo.FindPackageById(packageId);
            if (package == null)
                return null;   // caller can decide how to handle “not found”

            // Future business rules (e.g. price adjustments, feature flags) go here
            return package;
        }

        public async Task<List<Package>> GetAvailablePackagesAsync()
        {
            var packages = await _packageRepo.GetAvailablePackage();

            // Example business rule: hide packages with price == 0 (free trial hidden from list)
            // return packages.Where(p => p.Price > 0).ToList();

            return packages;
        }
    }
}
