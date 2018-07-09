using AutoMapper;

namespace vstupinfo.Common
{
    public class AutomapperConfig
    {

        public static void Configure()
        {
            Mapper.Initialize(cfg => { });
        }
    }
}