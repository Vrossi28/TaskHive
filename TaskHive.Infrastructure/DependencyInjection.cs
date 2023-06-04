using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Infrastructure.Persistence;

namespace TaskHive.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TaskHiveContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString(InfrastructureContants.ConnectionString),
                    builder => builder.MigrationsAssembly(typeof(TaskHiveContext).Assembly.FullName));
            });

            return services;
        }
    }
}
