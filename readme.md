# Dependency Graph Drawer
_aka `dotnet dg`_

[![Dependency Graph Drawer nuget package](https://img.shields.io/nuget/v/DependencyGraphDrawer)](https://www.nuget.org/packages/DependencyGraphDrawer)

This is the home of Dependency Graph Drawer, a global dotnet tool to visualize the dependencies in your C# projects.

## Installation

Run the following command: `dotnet tool install -g DependencyGraphDrawer`

Requires .NET 5.

## Usage

`dotnet dg -s <path to solution file> -o <output file path> [--includeNugetPackages]`

The resulting file will contain [PlantUML](https://plantuml.com/) code. You can preview out without installing anything by pasting the contents [here](http://www.plantuml.com/plantuml/uml/) but I recommend to use the [VS Code](https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml) plugin.