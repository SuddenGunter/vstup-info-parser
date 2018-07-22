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
    public class UniversityScrapper : IScrapper<University, Specialty>
    {
        public async Task<List<Specialty>> Scrap(University request)
        {
            try
            {
                Log.Debug("Acquiring specialties for {Name}", request.Name);
                var config = Configuration.Default.WithDefaultLoader();
                var page = await BrowsingContext.New(config).OpenAsync(request.Url);
                var queryRows = "#denna1 > tbody:nth-child(3) > tr";
                var queryInfo = " td:nth-child(1)";
                var queryUrls = " a";
                var queryName = $"{queryInfo} [title~=Спеціальність], {queryInfo} [title~=Факультет]";
                var rows = page.QuerySelectorAll(queryRows).ToList();
                var newDesign = rows.FirstOrDefault()?.QuerySelectorAll(queryName).FirstOrDefault() == null; //hardcoded fix for 2018y
                queryName = newDesign ?
                           $"{queryInfo} .search"
                           : queryName;

                var specialties = rows.Select(x =>
                {
                    var name = newDesign ?
                           x.QuerySelector(queryInfo).TextContent.Split("\n").ElementAt(4).Trim(' ')
                           : x.QuerySelectorAll(queryName).Select(e => e.TextContent).Aggregate((q, y) => q + "; Факультет: " + y);
                    
                    return new Specialty()
                    {
                        Info = x.QuerySelector(queryInfo).TextContent,
                        Name = name
                    };
                }).ToList();
                for (var i = 0; i < specialties.Count; i++)
                {
                    specialties.ElementAt(i).Url = rows.ElementAt(i).QuerySelector(queryUrls).GetHref();
                }
                var result = specialties.Where((x) => !String.IsNullOrEmpty(x.Url)).ToList();

                Log.Information("Got {Count} specialties for {Name}", result.Count, request.Name);
                Log.Debug("Here's complete list: ");
                result.ForEach(x => Log.Debug(x.Name));

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "University parsing error. University: {@request}", request);
                return new List<Specialty>();
            }
        }
    }
}