using Humanizer.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Infrastructure.Persistence
{
    public class TaskHiveDbContextFactory : IDesignTimeDbContextFactory<TaskHiveContext>
    {
        public TaskHiveContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, false)
                .Build();

            var builder = new DbContextOptionsBuilder<TaskHiveContext>();
            var connectionString = configuration.GetConnectionString(InfrastructureContants.ConnectionString);

            builder.UseSqlServer(connectionString);

            return new TaskHiveContext(builder.Options);
        }
    }
}
