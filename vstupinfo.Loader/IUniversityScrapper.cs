using System.Collections.Generic;
using System.Threading.Tasks;
using vstupinfo.Common.Models;

namespace vstupinfo.Loader
{
    public interface IUniversityScrapper
    {
        Task<IEnumerable<Specialty>> Scrap(University model);
    }
}