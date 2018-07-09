using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using vstupinfo.Common;
using vstupinfo.Common.Models;
using vstupinfo.Loader.Scrappers;

namespace vstupinfo.Loader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var services = new Config().BuildServices();
                var task = services.GetRequiredService<DownloadTask>();
                await task.Execute();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "App died cause of error");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}