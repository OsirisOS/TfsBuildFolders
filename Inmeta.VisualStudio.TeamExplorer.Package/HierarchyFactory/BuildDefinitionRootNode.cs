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

    internal class BuildDefinitionFolderNode : BuildDefinitionTreeNode
    {

        public BuildDefinitionFolderNode(string name, char separator)
            : base(name, separator)
        {
        }

    }

    internal class BuildDefinitionLeafNode : BuildDefinitionTreeNode
    {

        public bool Disabled { get; private set; }

        public BuildDefinitionLeafNode(string name, char separator,bool disabled)
            : base(name, separator)
        {
            Disabled = disabled;
        }

        public override bool IsDisabled
        {
            get
            {
                return Disabled;
            }
        }

        public override bool IsLeafNode
        {
            get { return true; }
        }

    }

    internal class BuildDefinitionFakeLeafNode : BuildDefinitionTreeNode
    {

        

        public BuildDefinitionFakeLeafNode(string name, char separator)
            : base(name, separator)
        {
        }

    }
}
