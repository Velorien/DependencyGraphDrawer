using System.IO;

namespace DependencyGraphDrawer
{
    record GraphOptions(FileInfo SolutionFile, FileInfo Output, bool IncludeNugetPackages);
}
