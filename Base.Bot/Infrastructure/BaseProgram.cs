using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.IO;

namespace Base.Bot.Infrastructure
{
    public static class BaseProgram
    {
        public static IWebHost BaseBuildWebHost<TStartup>(string[] args) where TStartup : class
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((WebHostBuilderContext builderContext, IConfigurationBuilder config) => {
                    var sharedFolder = Path.Combine(builderContext.HostingEnvironment.ContentRootPath, "..", "Shared");
                    var serverSettingsFolder = Path.Combine(sharedFolder, "ServerSettings");
                    var stagingEnvironment = GetStagingEnvironment(builderContext.HostingEnvironment);

                    config

                        .AddJsonFile("appsettings.json")
                        .AddJsonFile(GetBaseConf(stagingEnvironment))

                        .AddJsonFile(Path.Combine(serverSettingsFolder, GetServerDefaultConf(stagingEnvironment)), optional: true)
                        .AddJsonFile(GetServerDefaultConf(stagingEnvironment), optional: true)
                        
                        .AddJsonFile(Path.Combine(sharedFolder, "ErrorTranslations.json"), optional: true) // When running using dotnet run
                        .AddJsonFile("ErrorTranslations.json", optional: true) // When app is published

                        .AddJsonFile(Path.Combine(sharedFolder, "GeneralTranslations.json"), optional: true) // When running using dotnet run
                        .AddJsonFile("GeneralTranslations.json", optional: true); // When app is published
                        
                        
            })
                .CaptureStartupErrors(captureStartupErrors: true)
                .UseSetting("detailedErrors", "true")
                .UseStartup<TStartup>()
                .UseSerilog((WebHostBuilderContext hostingContext, LoggerConfiguration loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
        }

        private static string GetStagingEnvironment(IWebHostEnvironment env)
        {
            IConfigurationRoot configurationRoot = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .Build();

            return configurationRoot["StagingEnvironment"];
        }

        private static string GetBaseConf(string stagingEnvironment)
        {
            return $"appsettings.{stagingEnvironment}.json";
        }

        private static string GetServerDefaultConf(string stagingEnvironment)
        {
            return $"serversettings.{stagingEnvironment}.json";
        }
    }
}
