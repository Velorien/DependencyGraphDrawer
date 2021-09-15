using System.IO;

namespace DependencyGraphDrawer
{
    class GraphOptions
    {
        public FileInfo SolutionFile { get; set; }
        public FileInfo Output { get; set; }
        public bool IncludeNugetPackages { get; set; }
    }
}
