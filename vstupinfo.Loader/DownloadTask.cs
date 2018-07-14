using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using vstupinfo.Common.Models;
using vstupinfo.Loader.Scrappers;
using CsvHelper;

namespace vstupinfo.Loader
{
    public class DownloadTask
    {
        private readonly IScrapper<State, University> _universities;
        private readonly IScrapper<University, Specialty> _specialties;
        private readonly IScrapper<Specialty, Abiturient> _abiturients;
        public DownloadTask(IScrapper<State, University> universities,
                            IScrapper<University, Specialty> specialties,
                            IScrapper<Specialty, Abiturient> abiturients)
        {
            _universities = universities;
            _specialties = specialties;
            _abiturients = abiturients;
        }

        public async Task Execute(string url)
        {
            Log.Debug("Execution started");
            var state = new State()
            {
                Url = url,
                TableName = "vnzt0"
            };
            Log.Debug("State: {@state}", state);

            var universities = await _universities.Scrap(state);

            var tasksSpecs = new Task<List<Specialty>>[universities.Count];
            for (var i = 0; i < universities.Count; i++)
            {
                tasksSpecs[i] = _specialties.Scrap(universities[i]);
            }
            Task.WaitAll(tasksSpecs);

            var univerSpecs = new List<SpecialtyUniversity>();
            for (var i = 0; i < universities.Count; i++)
            {
                foreach (var spec in tasksSpecs[i].Result)
                {
                    univerSpecs.Add(new SpecialtyUniversity
                    {
                        Specialty = spec,
                        UniversityIndex = i
                    });
                }
            }
            Log.Information("Aquired {Count} specialties data!", univerSpecs.Count);

            var tasksAbiturs = new Task<List<Abiturient>>[univerSpecs.Count];
            for (var i = 0; i < univerSpecs.Count; i++)
            {
                tasksAbiturs[i] = _abiturients.Scrap(univerSpecs[i].Specialty);
            }
            Task.WaitAll(tasksAbiturs);

            var abitursSpecs = new List<AbiturientSpecialty>();
            for (var i = 0; i < univerSpecs.Count; i++)
            {
                foreach (var abitur in tasksAbiturs[i].Result)
                {
                    abitursSpecs.Add(new AbiturientSpecialty
                    {
                        Specialty = i,
                        Abiturient = abitur
                    });
                }
            }
            Log.Information("Aquired {Count} students data!", abitursSpecs.Count);

            var datetime = DateTime.Now;
            var uFileName = $"{datetime}-univer.csv"
                            .Replace("/", "-")
                            .Replace(@"\", "-")
                            .Replace(" ", "");
            Log.Information("Writing universities csv");
            using (TextWriter univerWriter = File.AppendText(uFileName))
            {
                var csv = new CsvWriter(univerWriter);
                csv.WriteRecords(universities);
                csv.Dispose();
            }
            Log.Information($"Saved at {uFileName}");

            var sFileName = $"{datetime}-specialties.csv"
                            .Replace("/", "-")
                            .Replace(@"\", "-")
                            .Replace(" ", "");
            Log.Information("Writing specialties csv");
            using (TextWriter specWriter = File.AppendText(sFileName))
            {
                var csv = new CsvWriter(specWriter);
                csv.WriteRecords(univerSpecs);
                csv.Dispose();
            }
            Log.Information($"Saved at {sFileName}");

            var aFileName = $"{datetime}-abiturs.csv"
                            .Replace("/", "-")
                            .Replace(@"\", "-")
                            .Replace(" ", "");
            Log.Information("Writing abiturients csv");
            using (TextWriter abitWriter = File.AppendText(aFileName))
            {
                var csv = new CsvWriter(abitWriter);
                csv.WriteRecords(abitursSpecs.Select(x => new
                {
                    abitur = x,
                    univer = universities[univerSpecs[x.Specialty].UniversityIndex].Name,
                    spec = univerSpecs[x.Specialty].Specialty.Name
                }));
                csv.Dispose();
            }
            Log.Information($"Saved at {aFileName}");


            Log.Information("Finished");
        }
    }
}