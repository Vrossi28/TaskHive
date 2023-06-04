using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TaskHive.Infrastructure;
using TaskHive.Infrastructure.Persistence;

namespace TaskHive.Application.Services.HealthCheck
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var connection = new TaskHiveContext();
                connection.Database.OpenConnection();
                connection.Database.ExecuteSqlRaw("SELECT 1");
                return HealthCheckResult.Healthy(); 
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(exception: ex);
            }
        }
    }
}
