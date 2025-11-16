using BusinessObject.Dtos.paymentDto;
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

        public async Task<PackageDetailDto> GetPackageDetailAsync(int packageId)
        {
            var package = await _packageRepo.FindPackageById(packageId, includeInactive: true);
            if (package == null)
                throw new Exception($"Package with ID {packageId} not found");

            var (totalPurchases, totalRevenue) = await _packageRepo.GetPackageStatistics(packageId);

            return new PackageDetailDto
            {
                Id = package.Id,
                Name = package.Name,
                CoinAmount = package.CoinAmount,
                Price = package.Price,
                Description = package.Description ?? "",
                IsActive = package.IsActive,
                TotalPurchases = totalPurchases,
                TotalRevenue = totalRevenue
            };
        }

        public async Task<List<PackageListDto>> GetAllPackagesAsync(bool includeInactive = true)
        {
            var packages = await _packageRepo.GetAllPackages(includeInactive);

            return packages.Select(p => new PackageListDto
            {
                Id = p.Id,
                Name = p.Name,
                CoinAmount = p.CoinAmount,
                Price = p.Price,
                Description = p.Description ?? "",
                IsActive = p.IsActive
            }).ToList();
        }

        public async Task<(List<PackageListDto> packages, int totalCount)> GetPaginatedPackagesAsync(
            int pageNumber, int pageSize, string? searchTerm = null, bool? isActive = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var (packages, totalCount) = await _packageRepo.GetPaginatedPackages(
                pageNumber, pageSize, searchTerm, isActive);

            var packageDtos = packages.Select(p => new PackageListDto
            {
                Id = p.Id,
                Name = p.Name,
                CoinAmount = p.CoinAmount,
                Price = p.Price,
                Description = p.Description ?? "",
                IsActive = p.IsActive
            }).ToList();

            return (packageDtos, totalCount);
        }

        public async Task<PackageDetailDto> CreatePackageAsync(CreatePackageDto createDto)
        {
            // Validate unique name
            var nameExists = await _packageRepo.PackageExistsByName(createDto.Name);
            if (nameExists)
                throw new Exception($"A package with name '{createDto.Name}' already exists");

            var package = new Package
            {
                Name = createDto.Name,
                CoinAmount = createDto.CoinAmount,
                Price = createDto.Price,
                Description = createDto.Description ?? "",
                IsActive = createDto.IsActive
            };

            var createdPackage = await _packageRepo.CreatePackage(package);

            return new PackageDetailDto
            {
                Id = createdPackage.Id,
                Name = createdPackage.Name,
                CoinAmount = createdPackage.CoinAmount,
                Price = createdPackage.Price,
                Description = createdPackage.Description ?? "",
                IsActive = createdPackage.IsActive,
                TotalPurchases = 0,
                TotalRevenue = 0
            };
        }

        public async Task<PackageDetailDto> UpdatePackageAsync(int packageId, UpdatePackageDto updateDto)
        {
            var package = await _packageRepo.FindPackageById(packageId, includeInactive: true);
            if (package == null)
                throw new Exception($"Package with ID {packageId} not found");

            // Update name if provided and validate uniqueness
            if (!string.IsNullOrWhiteSpace(updateDto.Name) && updateDto.Name != package.Name)
            {
                var nameExists = await _packageRepo.PackageExistsByName(updateDto.Name, packageId);
                if (nameExists)
                    throw new Exception($"A package with name '{updateDto.Name}' already exists");

                package.Name = updateDto.Name;
            }

            // Update other fields if provided
            if (updateDto.CoinAmount.HasValue)
                package.CoinAmount = updateDto.CoinAmount.Value;

            if (updateDto.Price.HasValue)
                package.Price = updateDto.Price.Value;

            if (updateDto.Description != null)
                package.Description = updateDto.Description;

            if (updateDto.IsActive.HasValue)
                package.IsActive = updateDto.IsActive.Value;

            var updatedPackage = await _packageRepo.UpdatePackage(package);
            var (totalPurchases, totalRevenue) = await _packageRepo.GetPackageStatistics(packageId);

            return new PackageDetailDto
            {
                Id = updatedPackage.Id,
                Name = updatedPackage.Name,
                CoinAmount = updatedPackage.CoinAmount,
                Price = updatedPackage.Price,
                Description = updatedPackage.Description ?? "",
                IsActive = updatedPackage.IsActive,
                TotalPurchases = totalPurchases,
                TotalRevenue = totalRevenue
            };
        }

        public async Task<bool> DeletePackageAsync(int packageId)
        {
            var package = await _packageRepo.FindPackageById(packageId, includeInactive: true);
            if (package == null)
                throw new Exception($"Package with ID {packageId} not found");

            return await _packageRepo.DeletePackage(packageId);
        }
    }
}
