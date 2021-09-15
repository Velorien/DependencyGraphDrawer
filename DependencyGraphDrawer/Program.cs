using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace DependencyGraphDrawer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand("A utility to create a PlantUML diagram of your C# project dependencies")
            {
                new Option<FileInfo>(new[] { "-s", "--solutionFile" })
                {
                    Description = "A path to your solution file"
                },
                new Option<FileInfo>(new [] { "-o", "--output" }, () => new FileInfo("dependencies.puml"))
                {
                    Description = "Output file location"
                },
                new Option<bool>("--includeNugetPackages", "Whether to include nuget packages in the diagram"),
            };

            rootCommand.Handler = CommandHandler.Create<GraphOptions>(GraphDrawer.DrawDiagram);

            await rootCommand.InvokeAsync(args);
        }
    }
}
