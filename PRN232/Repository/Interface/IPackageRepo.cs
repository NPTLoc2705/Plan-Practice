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
        Task<Package> FindPackageById(int packageId);
        Task<List<Package>> GetAvailablePackage();    
    }
}
