using System;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using vstupinfo.Common;
using vstupinfo.Common.Models;
using vstupinfo.Loader.Scrappers;

namespace vstupinfo.Loader
{
    public class Config
    {
        private readonly Options _opts;

        public Config(Options opts)
        {
            _opts = opts;
        }

        /// <summary>
        /// Setup DI
        /// </summary>
        public ServiceProvider BuildServices()
        {
            if (_opts.Debug)
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .CreateLogger();
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .CreateLogger();
            }
            Log.Debug("Building services");
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IScrapper<State, University>, StateScrapper>()
                .AddSingleton<IScrapper<University, Specialty>, UniversityScrapper>()
                .AddSingleton<IScrapper<Specialty, Abiturient>, SpecialtyScrapper>()
                .AddSingleton<DownloadTask>()
                .BuildServiceProvider();
            Log.Debug("Builded successfully");

            return serviceProvider;
        }
    }
}