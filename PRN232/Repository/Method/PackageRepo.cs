using BusinessObject.Payments;
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
        private readonly DAL.PackageDAO _packageDAO;
        public PackageRepo(DAL.PackageDAO packageDAO)
        {
            _packageDAO = packageDAO;
        }
        public async Task<Package> FindPackageById(int packageId)
        {
            return await _packageDAO.FindPackageById(packageId);
        }

        public async Task<List<Package>> GetAvailablePackage()
        => await _packageDAO.GetAllActivePackages();
    }
}
