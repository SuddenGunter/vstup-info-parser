using System.Collections.Generic;
using System.Threading.Tasks;

namespace vstupinfo.Loader.Scrappers
{
    /// <summary>
    /// Scrapper contract
    /// </summary>
    /// <typeparam name="TI">Input type</typeparam>
    /// <typeparam name="TO">Output type</typeparam>
    public interface IScrapper<TI, TO>
    {
        Task<List<TO>> Scrap(TI request);
    }
}