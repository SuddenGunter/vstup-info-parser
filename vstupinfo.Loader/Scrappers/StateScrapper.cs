using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom.Html;
using Serilog;
using vstupinfo.Common.Models;

namespace vstupinfo.Loader.Scrappers
{
    public class StateScrapper : IScrapper<State, University>
    {
        public async Task<List<University>> Scrap(State request)
        {
            Log.Debug("Acquiring universities");
            var config = Configuration.Default.WithDefaultLoader();
            var page = await BrowsingContext.New(config).OpenAsync(request.Url);
            var query = $"#{request.TableName} > tbody > tr > td > a";
            var cells = page.QuerySelectorAll(query);
            var universities = cells.Select(u => new University()
            {
                Name = u.TextContent,
                Url = u.GetHref()
            }).ToList();
            Log.Information("Got {Count} universities", universities.Count);
            Log.Debug("Here's complete list: ");
            universities.ForEach(x => Log.Debug(x.Name));

            return universities;
        }
    }
}