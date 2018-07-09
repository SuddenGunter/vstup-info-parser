using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using vstupinfo.Common.Models;

namespace vstupinfo.Loader
{
    public interface IStateScrapper
    {
        Task<IEnumerable<University>> Scrap(StateModel model);
    }
}