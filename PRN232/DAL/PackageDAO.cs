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

        public async Task<Package> FindPackageById(int packageId)
        {
            return await Task.FromResult(_context.Packages.FirstOrDefault(p => p.Id == packageId && p.IsActive));
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
    }
}
