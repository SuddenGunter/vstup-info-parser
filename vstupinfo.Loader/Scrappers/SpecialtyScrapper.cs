using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom.Html;
using Serilog;
using vstupinfo.Common.Models;

namespace vstupinfo.Loader.Scrappers
{
    public class SpecialtyScrapper : IScrapper<Specialty, Abiturient>
    {
        public async Task<List<Abiturient>> Scrap(Specialty request)
        {
            try
            {
                Log.Debug("Acquiring abiturients for {Name}", request.Name);
                var config = Configuration.Default.WithDefaultLoader();
                var page = await BrowsingContext.New(config).OpenAsync(request.Url);
                var queryRows = ".tablesaw.tablesaw-stack.tablesaw-sortable tbody tr:not(:empty)";
                var rows = page.QuerySelectorAll(queryRows).ToList();
                var result = new List<Abiturient>();
                foreach (var row in rows)
                {
                    var name = row.QuerySelector("td:nth-child(2)").TextContent.Split(' ');
                    var middleName = name.GetLength(0) > 2 ?
                                    name[2]
                                    : null;
                    var firstName = name.GetLength(0) > 1 ?
                                    name[1]
                                    : null;
                    var LastName = name.GetLength(0) >= 1 ?
                                    name[0]
                                    : null;

                    var priorityShift = row.Children.Count() > 8 ? 1 : 0; //3rd row is priority
                    var competitionValue = 4 + priorityShift;
                    var details = 5 + priorityShift;
                    var coefficients = 6 + priorityShift;
                    var quotas = 7 + priorityShift;
                    var documentOriginals = 8 + priorityShift;

                    result.Add(new Abiturient()
                    {
                        FirstName = firstName,
                        MiddleName = middleName,
                        LastName = LastName,
                        Details = row.QuerySelector($"td:nth-child({details})").TextContent,
                        Status = row.QuerySelector($"td:nth-child(3)").TextContent,
                        CompetitionValue =  row.QuerySelector($"td:nth-child({competitionValue})").TextContent,
                        DocumentOriginals = row.QuerySelector($"td:nth-child({documentOriginals})").TextContent,
                        Quotas = row.QuerySelector($"td:nth-child({quotas})").TextContent
                    });
                }

                Log.Information("Got {Count} abiturients for {Name}", rows.Count, request.Name);
                Log.Debug("Here's complete list: ");
                result.ForEach(x => Log.Debug($"{x.FirstName} {x.LastName}"));
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Specialty parsing error. Specialty: {@request}", request);
                return new List<Abiturient>();
            }
        }
    }
}