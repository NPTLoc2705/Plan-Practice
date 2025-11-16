using BusinessObject.Payments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class PackageDAO
    {
        private readonly PlantPraticeDbContext _context;

        public PackageDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }

        public async Task<Package?> FindPackageById(int packageId, bool includeInactive = false)
        {
            var query = _context.Packages
                .Include(p => p.Payments)
                .AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(p => p.IsActive);
            }

            return await query.FirstOrDefaultAsync(p => p.Id == packageId);
        }

        public async Task<List<Package>> GetAllActivePackages()
        {
            try
            {
                return await _context.Packages
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Price)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Package>> GetAllPackages(bool includeInactive = true)
        {
            try
            {
                var query = _context.Packages.AsQueryable();

                if (!includeInactive)
                {
                    query = query.Where(p => p.IsActive);
                }

                return await query
                    .OrderBy(p => p.Price)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<(List<Package> packages, int totalCount)> GetPaginatedPackages(
            int pageNumber, int pageSize, string? searchTerm = null, bool? isActive = null)
        {
            try
            {
                var query = _context.Packages.AsQueryable();

                // Filter by active status if specified
                if (isActive.HasValue)
                {
                    query = query.Where(p => p.IsActive == isActive.Value);
                }

                // Search by name or description
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(p =>
                        p.Name.Contains(searchTerm) ||
                        (p.Description != null && p.Description.Contains(searchTerm)));
                }

                var totalCount = await query.CountAsync();

                var packages = await query
                    .OrderBy(p => p.Price)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (packages, totalCount);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Package> CreatePackage(Package package)
        {
            try
            {
                _context.Packages.Add(package);
                await _context.SaveChangesAsync();
                return package;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Package> UpdatePackage(Package package)
        {
            try
            {
                _context.Packages.Update(package);
                await _context.SaveChangesAsync();
                return package;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeletePackage(int packageId)
        {
            try
            {
                var package = await _context.Packages.FindAsync(packageId);
                if (package == null)
                    return false;

                // Check if package has any payments
                var hasPayments = await _context.Payments
                    .AnyAsync(p => p.PackageId == packageId);

                if (hasPayments)
                {
                    // Soft delete - just mark as inactive
                    package.IsActive = false;
                    _context.Packages.Update(package);
                }
                else
                {
                    // Hard delete if no payments
                    _context.Packages.Remove(package);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> PackageExistsByName(string name, int? excludeId = null)
        {
            var query = _context.Packages.Where(p => p.Name == name);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<(int totalPurchases, int totalRevenue)> GetPackageStatistics(int packageId)
        {
            try
            {
                var stats = await _context.Payments
                    .Where(p => p.PackageId == packageId && p.Status == "PAID")
                    .GroupBy(p => p.PackageId)
                    .Select(g => new
                    {
                        TotalPurchases = g.Count(),
                        TotalRevenue = g.Sum(p => p.Amount)
                    })
                    .FirstOrDefaultAsync();

                return (stats?.TotalPurchases ?? 0, stats?.TotalRevenue ?? 0);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}