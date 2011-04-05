using System;
using System.ComponentModel.Design;
using System.Drawing;
using Inmeta.VisualStudio.TeamExplorer.HierarchyFactory;
using Inmeta.VisualStudio.TeamExplorer.Plugin;
using Inmeta.VisualStudio.TeamExplorer.ToolsOptions;
using Microsoft.TeamFoundation.Common;
using Fasterflect;
using Microsoft.VisualStudio.Shell.Interop;

namespace Inmeta.VisualStudio.TeamExplorer.ExplorerNodes
{
    public class BuildDefinitionExplorerNode : BaseHierarchyNode, ICommandableNode
    {
        private readonly char separator;

        private static IntPtr _hiconLeaf;
        private static IntPtr _hiconOpenFolder;
        private static IntPtr _hiconCloseFolder;
        
        public override object GetProperty(int propId)
        {
            //get correct image.
            switch (((__VSHPROPID)propId))
            {
                case __VSHPROPID.VSHPROPID_IconHandle:
                    {
                        if (NodePriority == (int) TeamExplorerNodePriority.Leaf)
                        {
                            return _hiconLeaf.ToInt32(); 
                        }
                        return _hiconCloseFolder.ToInt32(); 
                    }

                case __VSHPROPID.VSHPROPID_OpenFolderIconHandle:
                    {
                        if (NodePriority == (int) TeamExplorerNodePriority.Folder)
                        {
                            return _hiconOpenFolder.ToInt32();
                        }

                        return null;
                    }
            }
            return base.GetProperty(propId);

        }

        public override void DoDefaultAction()
        {
            //get all build definitions if not leaf.
            if (NodePriority == (int)TeamExplorerNodePriority.Leaf)
                FindAssociated.AssociatedNode(this, separator).DoDefaultAction();
        }

        public void OpenEditBuildDefintion()
        {
            var node = FindAssociated.AssociatedNode(this, separator);
            node.ParentHierarchy.CallMethod("OpenBuildDefinition", node);
        }

        public void QueueNewBuild()
        {
            //get all build definitions if not leaf.
            BaseHierarchyNode node = NodePriority != (int)TeamExplorerNodePriority.Leaf ? FindAssociated.AssociatedNode("All Build Definitions", this) : FindAssociated.AssociatedNode(this, separator);

            node.ParentHierarchy.CallMethod("QueueBuild", node);
        }

        public void ViewBuilds()
        {
            //get all build definitions if not leaf.
            BaseHierarchyNode node = NodePriority != (int)TeamExplorerNodePriority.Leaf ? FindAssociated.AssociatedNode("All Build Definitions", this) : FindAssociated.AssociatedNode(this, separator);

            node.ParentHierarchy.CallMethod("ViewBuilds", node);
        }

        public void Options()
        {
            using (var options = new ToolsOptions.OptionsForm(ParentHierarchy))
            {
                options.ShowDialog();
            }
        }

        public BuildDefinitionExplorerNode(IBuildDefinitionTreeNode node, char separator)
            : base(node.Path, node.Name)
        {

            if (_hiconLeaf == IntPtr.Zero && _hiconOpenFolder == IntPtr.Zero)
            {
               _hiconLeaf = ((Bitmap)BuildExplorerPlugin.Icons.Images[2]).GetHicon();
               _hiconOpenFolder =  ((Bitmap) BuildExplorerPlugin.Icons.Images[4]).GetHicon();
               _hiconCloseFolder = ((Bitmap)BuildExplorerPlugin.Icons.Images[3]).GetHicon();   
            }

            this.separator = separator;
            if (node.Children.Count == 0)
            {
                InitAsLeaf();
            }
            else
            {
                InitAsFolder();
            }
        }

        public override CommandID ContextMenu
        {
            get
            {
                return GuidList.ContextMenuCommand;
            }
        }

        public override string PropertiesClassName
        {
            get { return "Build Definition Node"; }
        }

    }

}
