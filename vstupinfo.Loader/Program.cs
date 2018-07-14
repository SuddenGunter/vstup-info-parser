using System;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
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
                var opts = CommandLine.Parser.Default.ParseArguments<Options>(args);
                var optionsValues = opts.MapResult(x => x, errs =>
                {
                    errs.ToList().ForEach(t => Console.WriteLine(t.ToString()));
                    return null;
                });
                if (optionsValues != null)
                {
                    await RunOptionsAndReturnExitCode(optionsValues);
                }
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

        private async static Task RunOptionsAndReturnExitCode(Options opts)
        {
            var services = new Config(opts).BuildServices();
            var task = services.GetRequiredService<DownloadTask>();
            await task.Execute(opts.Url);
        }

        private static void HandleParseError(object errs)
        {
            Console.WriteLine(errs.ToString());
        }
    }
}