using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Core.Entities;
using TaskHive.Infrastructure.Persistence;

namespace TaskHive.Infrastructure.Repositories
{
    public class CompanyRepository
    {
        private TaskHiveContext _dbContext = new();

        public async Task<Company> GetCompanyById(Guid companyId)
        {
            Company company = null;

            company = await _dbContext.Company.AsNoTracking().SingleOrDefaultAsync((a) => a.CompanyId == companyId && a.CompanyState == Core.Enums.State.Active);

            return company;
        }

        public List<Company> GetCompaniesByName(string name)
        {
            List<Company> companies = new();

            companies = _dbContext.Company.AsNoTracking().Where((c) => c.Name == name).ToList();

            return companies;
        }

        public async Task<bool> AddCompany(Company company)
        {
            _dbContext.Company.Add(company);
            var result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }
    }
}
