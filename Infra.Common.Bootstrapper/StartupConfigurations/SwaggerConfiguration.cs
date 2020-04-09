using Infra.Common.Bootstrapper.StartupSettings;
using Framework.Core.Extentions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Framework.Web.Settings;
using System.Linq;

namespace Infra.Common.Bootstrapper.StartupConfigurations
{
    public static class SwaggerConfiguration
    {
        public static void AddCustomSwagger(this IServiceCollection services, Settings settings)
        {

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(settings.Swagger.Info.Version, new OpenApiInfo
                {
                    Version = settings.Swagger.Info.Version,
                    Title = settings.Swagger.Info.Title,
                    Description = settings.Swagger.Info.Description,
                    Contact = new OpenApiContact() { Name = settings.Swagger.Info.Contact.Name, Email = settings.Swagger.Info.Contact.Email, Url = new Uri(settings.Swagger.Info.Contact.Url) }
                });
               
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });

                var xmlFile = settings.Swagger.XmlFile.Name;
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);

            });

        }

    }
}
