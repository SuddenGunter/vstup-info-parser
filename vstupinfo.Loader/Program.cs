using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using vstupinfo.Common;
using vstupinfo.Common.Models;

namespace vstupinfo.Loader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //setup our DI
            var serviceProvider = new ServiceCollection()
                
                .BuildServiceProvider();
          
            AutomapperConfig.Configure();

            (await new StateScrapper().Scrap(new StateModel()
            {
                Url = "http://vstup.info/2018/i2018o21.html",
                TableName = "vnzt0"
            })).ToList().ForEach(x =>
            {
                Console.WriteLine(x.Name);
            });
        }
    }
}