using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom.Html;
using vstupinfo.Common.Models;

namespace vstupinfo.Loader
{
    public class StateScrapper : IStateScrapper
    {
        public async Task<IEnumerable<University>> Scrap(StateModel model)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var page = await BrowsingContext.New(config).OpenAsync(model.Url);
            var query = $"#{model.TableName} > tbody > tr > td > a";
            var cells = page.QuerySelectorAll(query);
            return cells.AsParallel().Select(m => new University()
            {
                Name = m.TextContent,
                Url = ((IHtmlAnchorElement) (m)).Href
            });
        }
    }
}