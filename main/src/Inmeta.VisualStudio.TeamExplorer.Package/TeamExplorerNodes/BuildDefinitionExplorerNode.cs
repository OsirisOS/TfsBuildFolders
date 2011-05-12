using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Net;
using Inmeta.VisualStudio.TeamExplorer.HierarchyFactory;
using Inmeta.VisualStudio.TeamExplorer.Plugin;
using Inmeta.VisualStudio.TeamExplorer.ToolsOptions;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Fasterflect;
using Microsoft.TeamFoundation.VersionControl.Client;
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
                        if (NodePriority == (int)TeamExplorerNodePriority.Leaf)
                        {
                            return _hiconLeaf.ToInt32();
                        }
                        return _hiconCloseFolder.ToInt32();
                    }
                case __VSHPROPID.VSHPROPID_OverlayIconIndex:
                    {
                        if (NodePriority == (int)TeamExplorerNodePriority.Leaf && IsDisabled)
                        {
                            //disabled icon.
                            return 6;
                        }
                        return null;
                    }
                case __VSHPROPID.VSHPROPID_OpenFolderIconHandle:
                    {
                        if (NodePriority == (int)TeamExplorerNodePriority.Folder)
                        {
                            return _hiconOpenFolder.ToInt32();
                        }

                        return null;
                    }
            }
            return base.GetProperty(propId);

        }

        public bool IsDisabled { get; set; }

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

        public void GotoTeamExplorerBuildNode()
        {
            var node = FindAssociated.AssociatedNode(this, separator);
            node.Select();
            
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

            IsDisabled = node.IsDisabled;
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


        public void ViewAllBuilds()
        {
            // ignore here, should never be called here
        }

        public void QueueDefaultSubFolderBuilds()
        {
           // if (NodePriority == (int)TeamExplorerNodePriority.Leaf)
           //     return;  // just ignore, we should never be here
            var buildServer = GetBuildServer();
            foreach (var buildDef in
                FindAssociated.AssociatedNodes(this.CanonicalName, this).Select(item => buildServer.GetBuildDefinition(item.ProjectName, item.Name)))
            {
                buildServer.QueueBuild(buildDef);
            }
            var node = FindAssociated.AssociatedNode("All Build Definitions", this);
            node.ParentHierarchy.CallMethod("ViewBuilds", node);
        }



        private IBuildServer GetBuildServer()
        {
            var authenticatedTFS = new AuthTfsTeamProjectCollection(ParentHierarchy.ServerUrl);
            return authenticatedTFS.TfsBuildServer;
        }
    }

}
