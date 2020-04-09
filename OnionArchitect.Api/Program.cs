using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Framework.Web.Extentions;
using Framework.Web.Filters;
using Infra.Common.Bootstrapper.StartupConfigurations;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Sinks.MSSqlServer;

namespace OnionArchitect.Api
{

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .Enrich.FromLogContext()
                        .WriteLogRequestsToSqlServer(ReadConnectionString("LogRequest"))
                        .WriteLogsToSqlServer(ReadConnectionString("Log"));
                });
        private static string ReadConnectionString(string name)
        {
            var currentDir = Directory.GetCurrentDirectory();
            var token = JToken.Parse(System.IO.File.ReadAllText(currentDir + "\\appsettings.json"));
            var cs = token["ConnectionStrings"][name].ToString();


            return cs;
        }

       
    }

}
