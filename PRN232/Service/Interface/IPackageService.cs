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
    }
}
