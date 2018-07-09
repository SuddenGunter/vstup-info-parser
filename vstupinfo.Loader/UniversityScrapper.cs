using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom.Html;
using vstupinfo.Common.Models;

namespace vstupinfo.Loader
{
    class UniversityScrapper : IUniversityScrapper
    {       
        public async Task<IEnumerable<Specialty>> Scrap(University model)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var page = await BrowsingContext.New(config).OpenAsync(model.Url);
            var queryRows = $"#denna1 > tbody > tr > td > span > a";
            var links = page.QuerySelectorAll(queryRows).AsParallel();
            
            return links.AsParallel().Select(m => new Specialty()
            {
                Info = m.TextContent,
                Url = ((IHtmlAnchorElement) (m)).Href
            });
        }
    }
}