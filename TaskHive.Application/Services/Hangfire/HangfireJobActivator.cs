using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Application.Services.Hangfire
{
    public class HangfireJobActivator : JobActivator
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public HangfireJobActivator(IServiceProvider serviceProvider)
        {
            _serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        }

        public override object ActivateJob(Type type)
        {
            var scope = _serviceScopeFactory.CreateScope();
            return scope.ServiceProvider.GetService(type);
        }
    }
}
