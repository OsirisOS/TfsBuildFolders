using System.Collections.ObjectModel;

namespace Inmeta.VisualStudio.TeamExplorer.HierarchyFactory
{
    public interface IBuildDefinitionTreeNode  
    {
        ReadOnlyCollection<IBuildDefinitionTreeNode> Children { get; }
        string Name { get; }
        IBuildDefinitionTreeNode Parent { get; }
        string Path { get; }
        bool IsRoot { get; }
    }
}