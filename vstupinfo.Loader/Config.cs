using Microsoft.Extensions.DependencyInjection;
using Serilog;
using vstupinfo.Common;
using vstupinfo.Common.Models;
using vstupinfo.Loader.Scrappers;

namespace vstupinfo.Loader
{
    public class Config
    {
        /// <summary>
        /// Setup DI
        /// </summary>
        public ServiceProvider BuildServices()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();

            Log.Debug("Building services");
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IScrapper<State, University>, StateScrapper>()
                .AddSingleton<IScrapper<University, Specialty>, UniversityScrapper>()
                .AddScoped<DownloadTask>()
                .BuildServiceProvider();
            Log.Debug("Builded successfully");

            return serviceProvider;
        }
    }
}