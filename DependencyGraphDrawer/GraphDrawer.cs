using Microsoft.Build.Construction;
using Microsoft.Build.Exceptions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace DependencyGraphDrawer
{
    static class GraphDrawer
    {
        public static void DrawDiagram(GraphOptions options)
        {
            if (options.SolutionFile is not null && !options.SolutionFile.Exists)
            {
                System.Console.WriteLine("The specified file does not exist");
                return;
            }

            if (options.SolutionFile is null)
            {
                var slns = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.sln");
                if (slns.Any())
                {
                    options = options with { SolutionFile = new FileInfo(slns.First()) };
                }
                else
                {
                    System.Console.WriteLine("Cannot find a solution file");
                    return;
                }
            }

            SolutionFile solution = null;
            try
            {
                solution = SolutionFile.Parse(options.SolutionFile.FullName);
            }
            catch (InvalidProjectFileException)
            {
                System.Console.WriteLine("The file provided is not a solution file.");
                return;
            }

            var sb = new StringBuilder();
            string nugetColor = "#ecf0f1";
            string projectColor = "#3498db";
            if (!options.IncludeNugetPackages) projectColor = nugetColor;

            sb.AppendLine("@startuml Dependency graph");
            sb.AppendLine();
            sb.AppendLine("!theme plain");
            sb.AppendLine("skinparam ComponentStyle rectangle");
            sb.AppendLine("left to right direction");
            sb.AppendLine();

            var projects = solution.ProjectsInOrder.Where(x =>
            {
                if (x.ProjectType is SolutionProjectType.SolutionFolder
                                  or SolutionProjectType.EtpSubProject
                                  or SolutionProjectType.WebDeploymentProject)
                {
                    return false;
                }

                try
                {
                    var attributes = File.GetAttributes(x.AbsolutePath);
                    return !attributes.HasFlag(FileAttributes.Directory);
                }
                catch (FileNotFoundException)
                {
                    System.Console.WriteLine("File not found: " + x.AbsolutePath);
                    return false;
                }
            });

            sb.AppendLine("' Projects");
            foreach (var project in projects)
            {
                sb.AppendLine($"[{project.ProjectName}] {projectColor}");
            }
            sb.AppendLine();

            if (options.IncludeNugetPackages)
            {
                sb.AppendLine("' Nuget packages");
                var nugets = new HashSet<string>();
                foreach (var project in projects)
                {
                    var csproj = new XmlDocument();
                    csproj.Load(project.AbsolutePath);

                    foreach (XmlNode dependency in csproj.GetElementsByTagName("PackageReference"))
                    {
                        var name = dependency.Attributes["Include"].Value;
                        var version = dependency.Attributes["Version"]?.Value;
                        if (version is null) version = dependency.Value;
                        nugets.Add($"[{name} v{version}] as {GetNugetId(name, version)} {nugetColor}");
                    }
                }

                foreach (var nuget in nugets)
                {
                    sb.AppendLine(nuget);
                }
                sb.AppendLine();
            }

            foreach (var project in projects)
            {
                var attributes = File.GetAttributes(project.AbsolutePath);
                if (attributes.HasFlag(FileAttributes.Directory)) continue;

                var csproj = new XmlDocument();
                csproj.Load(project.AbsolutePath);

                foreach (XmlNode dependency in csproj.GetElementsByTagName("ProjectReference"))
                {
                    var projectName = dependency.Attributes["Include"].Value.Split("\\")[^1].Replace(".csproj", string.Empty);
                    sb.AppendLine($"{projectName} --> {project.ProjectName}");
                }

                if (options.IncludeNugetPackages)
                {
                    foreach (XmlNode dependency in csproj.GetElementsByTagName("PackageReference"))
                    {
                        var name = dependency.Attributes["Include"].Value;
                        var version = dependency.Attributes["Version"]?.Value;
                        if (version is null) version = dependency.Value;
                        sb.AppendLine($"{GetNugetId(name, version)} --> {project.ProjectName}");
                    }
                }
            }

            sb.AppendLine("@enduml");
            File.WriteAllText(options.Output.FullName, sb.ToString());
        }

        private static string GetNugetId(string name, string version) => "_" + (name + " v" + version).GetHashCode().ToString().Replace("-", "_");
    }
}
