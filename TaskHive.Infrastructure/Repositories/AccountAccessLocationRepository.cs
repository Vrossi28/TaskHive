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
    public class AccountAccessLocationRepository
    {
        private TaskHiveContext _dbContext = new();

        public async Task<bool> AddAccessLocation(AccountAccessLocation accessLocation)
        {
            _dbContext.AccountAccessLocation.Add(accessLocation);
            var result = await _dbContext.SaveChangesAsync();

            return Convert.ToBoolean(result);
        }

        public List<AccountAccessLocation> GetAccessLocationByAccount(Guid accountId)
        {
            List<AccountAccessLocation> accessLocations = new();

            accessLocations = _dbContext.AccountAccessLocation.AsNoTracking().Where((e) => e.AccountId == accountId).ToList();

            return accessLocations;
        }
    }
}
