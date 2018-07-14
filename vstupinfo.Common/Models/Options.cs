using CommandLine;

namespace vstupinfo.Common.Models
{
    public class Options
    {
        [Option(Required = true, HelpText = "Url to work with.")]
        public string Url { get; set; }

        [Option(Default = false, HelpText = "Prints debug output.")]
        public bool Debug { get; set; }
    }
}