using Infra.Common.Bootstrapper.StartupSettings;
using Infra.Data.SqlServer.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infra.Common.Bootstrapper.StartupConfigurations
{
    public static class OnionArchitectDbContextConfiguration
    {
        public static void AddCustomDbContext(this IServiceCollection services, Settings settings)
        {
            services.AddDbContextPool<OnionArchitectDbContext>(options =>
            {
                options
                    .UseSqlServer(settings.ConnectionStrings.Domain)
                    //Tips
                    .ConfigureWarnings(warning => warning.Throw(RelationalEventId.QueryClientEvaluationWarning));
            });
        }
    }
}
