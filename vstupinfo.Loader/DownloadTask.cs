using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using vstupinfo.Common.Models;
using vstupinfo.Loader.Scrappers;

namespace vstupinfo.Loader
{
    public class DownloadTask
    {
        private readonly IScrapper<State, University> _universities;
        private readonly IScrapper<University, Specialty> _specialties;

        public DownloadTask(IScrapper<State, University> universities,
                            IScrapper<University, Specialty> specialties)
        {
            _universities = universities;
            _specialties = specialties;
        }

        public async Task Execute()
        {
            Log.Debug("Execution started");
            var state = new State()
            {
                Url = "http://vstup.info/2017/i2017o21.html",
                TableName = "vnzt0"
            };
            Log.Information("State: {@state}", state);

            var universities = await _universities.Scrap(state);
            Log.Debug("Memory usage: {0}", (GC.GetTotalMemory(false) / 1024d) / 1024d);
/*
            universities.AsParallel().ForAll(async u =>
                 await _specialties.Scrap(u)
            );*/
            var u = universities.ElementAt(1);
                 await _specialties.Scrap(u);
             u = universities.ElementAt(4);
                 await _specialties.Scrap(u);
             u = universities.ElementAt(10);
                 await _specialties.Scrap(u);

            Log.Debug("Memory usage: {0}", (GC.GetTotalMemory(false) / 1024d) / 1024d);

            Log.Information("Finished");
        }
    }
}