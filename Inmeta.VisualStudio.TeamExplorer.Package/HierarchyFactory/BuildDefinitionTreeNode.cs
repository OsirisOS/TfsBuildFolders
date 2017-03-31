using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Shell.Interop;

namespace Inmeta.VisualStudio.TeamExplorer.HierarchyFactory
{
    [UserContext(VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_LookupF1, "keyword", "vs.tfc.teamprojectexplorer_teambuildsnode")]
    abstract internal class BuildDefinitionTreeNode : IBuildDefinitionTreeNode
    {
        private readonly char separator;
        protected List<IBuildDefinitionTreeNode> _children = new List<IBuildDefinitionTreeNode>();

        public ReadOnlyCollection<IBuildDefinitionTreeNode> Children { get { return new ReadOnlyCollection<IBuildDefinitionTreeNode>(_children); } }
        
        public virtual string Name { get; private set;}

        public virtual bool IsRoot { get { return false; } } 

        public virtual string Path { 
            get 
            { 
                var path= new List<IBuildDefinitionTreeNode>();
                var parent = (IBuildDefinitionTreeNode)this;
                
                //do not aggregate root into path name.
                while (parent != null && !parent.IsRoot)
                {
                    path.Insert(0, parent);
                    parent = parent.Parent;
                }

                var temp = path.Aggregate("", (seed, node) => seed += separator + node.Name);

                //remove first [InmetaBuildExplorerSettings.Default.SeparatorToken]
                return  temp.Length > 0 ? temp.Remove(0, 1) : string.Empty;

            }
        }

        protected BuildDefinitionTreeNode(string name, char separator)
        {
            this.separator = separator;
            Name = name;
        }

        public virtual bool IsLeafNode
        {
            get { return false; }
        }

        internal void AddChild(IBuildDefinitionTreeNode child)
        {
            ((BuildDefinitionTreeNode) child).Parent = this;
            _children.Add(child);
        }

        public IBuildDefinitionTreeNode Parent { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public virtual bool IsDisabled
        {
            get { return false; }
        }
    }
}
