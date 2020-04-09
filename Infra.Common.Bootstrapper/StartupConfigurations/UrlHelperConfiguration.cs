using Framework.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infra.Common.Bootstrapper.StartupConfigurations
{
    public static class UrlHelperConfiguration
    {
        public static void AddCustomUrlHelper(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper, UrlHelper>(implementationFactory => {
                var actionContext =
                implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);

            });
            services.AddScoped<PagingMetadataHelper>(implementationFactory => {
                var urlHelper =
                implementationFactory.GetService<IUrlHelper>();
                return new PagingMetadataHelper(urlHelper);

            });
            

            services.AddSingleton<IWebHelper, WebHelper>();
        }
    }
}
