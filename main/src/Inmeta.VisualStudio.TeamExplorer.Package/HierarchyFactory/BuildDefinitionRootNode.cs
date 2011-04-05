namespace Inmeta.VisualStudio.TeamExplorer.HierarchyFactory
{
    internal class BuildDefinitionRootNode : BuildDefinitionTreeNode
    {

        public override bool IsRoot { get { return true; } }

        public BuildDefinitionRootNode(string name, char separator)
            : base(name, separator)
        {
        }

    }
}
