using System;
using System.Collections.Generic;

namespace DependencyGraphDrawer
{
    abstract record DependencyItem
    {
        //public HashSet<DependencyItem> Dependencies { get; } = new();

        public abstract string GetDisplayName();
        public string GetId() => $"_{GetHashCode().ToString().Replace("-", "_")}";
        public string GetDefinition() => $"component \"{GetDisplayName()}\" as {GetId()}";
    }

    record ProjectItem(string Name) : DependencyItem
    {
        public override string GetDisplayName() => Name;
    }

    record NugetPackageItem(string Name, string Version) : DependencyItem
    {
        public override string GetDisplayName() => $"{Name}, {Version}";
    }
}
