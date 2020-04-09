using Framework.Web.Settings;
using Infra.Common.Bootstrapper.StartupSettings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infra.Common.Bootstrapper.StartupConfigurations
{
    public static class ApiVersioningConfiguration
    {
        public static void AddCustomApiVersioning(this IServiceCollection services, Settings settings)
        {
            services.AddApiVersioning(options => {
                options.AssumeDefaultVersionWhenUnspecified = settings.ApiVertioning.AssumeDefaultVersionWhenUnspecified; //default is false
                options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(settings.ApiVertioning.DefaultApiVersion, 0); //v1.0 == v1

            });
            //url segment => {version}
        }
    }
}
