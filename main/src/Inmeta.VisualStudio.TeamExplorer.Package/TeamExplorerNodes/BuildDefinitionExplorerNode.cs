using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using Fasterflect;
using Inmeta.VisualStudio.TeamExplorer.HierarchyFactory;
using Inmeta.VisualStudio.TeamExplorer.Plugin;
using Inmeta.VisualStudio.TeamExplorer.TeamExplorerNodes;
using Inmeta.VisualStudio.TeamExplorer.ToolsOptions;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Shell.Interop;

namespace Inmeta.VisualStudio.TeamExplorer.ExplorerNodes
{
    public class BuildDefinitionExplorerNode : BaseHierarchyNode, ICommandableNode
    {
        private const int DisableSign = 6;
        private static IntPtr _hiconLeaf;
        private static IntPtr _hiconLeafAllBuildDef;
        private static IntPtr _hiconOpenFolder;
        private static IntPtr _hiconCloseFolder;
        private readonly bool isFolderNode;
        private readonly char separator;
        private IBuildDefinitionTreeNode Node;
        private const int AllBuildDefinitionsNodePriority = 50;

        public BuildDefinitionExplorerNode(IBuildDefinitionTreeNode node, char separator, bool isFolderNode, ITFS tfs)
            : this(node, separator,tfs)
        {
            this.isFolderNode = isFolderNode;
        }

        private ITFS TFS;

        public BuildDefinitionExplorerNode(IBuildDefinitionTreeNode node, char separator,ITFS tfs)
            : base(node.Path, node.Name)
        {
            TFS = tfs;
            Node = node;
            if (_hiconLeaf == IntPtr.Zero && _hiconOpenFolder == IntPtr.Zero)
            {
                _hiconLeafAllBuildDef = ((Bitmap) BuildExplorerPlugin.Icons.Images[0]).GetHicon();
                _hiconLeaf = ((Bitmap) BuildExplorerPlugin.Icons.Images[2]).GetHicon();
                _hiconOpenFolder = ((Bitmap) BuildExplorerPlugin.Icons.Images[4]).GetHicon();
                _hiconCloseFolder = ((Bitmap) BuildExplorerPlugin.Icons.Images[3]).GetHicon();
            }

            this.separator = separator;
            if (node.Children.Count == 0 /*&& node.Name!="All Build Definitions"*/)
            {
                InitAsLeaf();
            }
            else
            {
                InitAsFolder();
            }
            IsAllBuildDefinitionsNode = node.Name == "All Build Definitions";
            if (IsAllBuildDefinitionsNode)
            {
                NodePriority = AllBuildDefinitionsNodePriority;
            }
            IsDisabled = node.IsDisabled;
        }

        public void AddChildren()
        {
            foreach (var buildNode in Node.Children.Select(node => new BuildDefinitionExplorerNode(node, separator,TFS)))
            {
                buildNode.AddChildren();
                AddChild(buildNode);
            }
        }


        public bool IsAllBuildDefinitionsNode { get; private set; }

        public override CommandID ContextMenu
        {
            get { return GuidList.ContextMenuCommand; }
        }

        public override string PropertiesClassName
        {
            get { return "Build Definition Node"; }
        }

        #region ICommandableNode Members

        public bool IsDisabled { get; set; }

        public override void DoDefaultAction()
        {
            //get all build definitions if not leaf.
            if (NodePriority == (int) TeamExplorerNodePriority.Leaf || NodePriority == AllBuildDefinitionsNodePriority)
                FindAssociated.AssociatedNode(this, separator).DoDefaultAction();
        }

        public void OpenEditBuildDefintion()
        {
            BaseHierarchyNode node = FindAssociated.AssociatedNode(this, separator);
            node.ParentHierarchy.CallMethod("OpenBuildDefinition", node);
        }

        public void GotoTeamExplorerBuildNode()
        {
            BaseHierarchyNode node = FindAssociated.AssociatedNode(this, separator);
            node.Select();
        }

        public void QueueNewBuild()
        {
            //get all build definitions if not leaf.
            BaseHierarchyNode node = NodePriority != (int) TeamExplorerNodePriority.Leaf
                                         ? FindAssociated.AssociatedNode("All Build Definitions", this, separator)
                                         : FindAssociated.AssociatedNode(this, separator);

            if (!(NodePriority == (int) TeamExplorerNodePriority.Leaf && IsDisabled))
                node.ParentHierarchy.CallMethod("QueueBuild", node);
        }

        public void ViewBuilds()
        {
            //get all build definitions if not leaf.
            BaseHierarchyNode node = NodePriority != (int) TeamExplorerNodePriority.Leaf
                                         ? FindAssociated.AssociatedNode("All Build Definitions", this, separator)
                                         : FindAssociated.AssociatedNode(this, separator);

            node.ParentHierarchy.CallMethod("ViewBuilds", node);
        }

        public void Options()
        {
            using (var options = new OptionsForm(ParentHierarchy,TFS))
            {
                options.ShowDialog();
            }
        }


        public void ViewAllBuilds()
        {
            BaseHierarchyNode node = FindAssociated.AssociatedNode("All Build Definitions", this, separator);
            node.ParentHierarchy.CallMethod("ViewBuilds", node);
        }

        public void QueueDefaultSubFolderBuilds()
        {
            // if (NodePriority == (int)TeamExplorerNodePriority.Leaf)
            //     return;  // just ignore, we should never be here
            IBuildServer buildServer = GetBuildServer();
            foreach (IBuildDefinition buildDef in
                FindAssociated.AssociatedNodes(CanonicalName, this, separator).Select(
                    item => buildServer.GetBuildDefinition(item.ProjectName, item.Name)))
            {
                if (buildDef.Enabled)
                    buildServer.QueueBuild(buildDef);
            }
            BaseHierarchyNode node = FindAssociated.AssociatedNode("All Build Definitions", this, separator);
            node.ParentHierarchy.CallMethod("ViewBuilds", node);
        }


        public bool IsFolderNode
        {
            get { return isFolderNode; }
        }

        #endregion

        public override object GetProperty(int propId)
        {
            //get correct image.
            switch (((__VSHPROPID) propId))
            {
                case __VSHPROPID.VSHPROPID_IconHandle:
                    {
                        if (NodePriority == (int) TeamExplorerNodePriority.Leaf)
                            return _hiconLeaf.ToInt32();
                        return IsAllBuildDefinitionsNode ? _hiconLeafAllBuildDef.ToInt32() : _hiconCloseFolder.ToInt32();
                    }
                case __VSHPROPID.VSHPROPID_OverlayIconIndex:
                    {
                        if (NodePriority == (int) TeamExplorerNodePriority.Leaf && IsDisabled)
                        {
                            //disabled icon.
                            return DisableSign;
                        }
                        return null;
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

        private IBuildServer GetBuildServer()
        {
            var authenticatedTFS = new AuthTfsTeamProjectCollection(TFS);                   // was formerly (ParentHierarchy.ServerUrl);
            return authenticatedTFS.TfsBuildServer;
        }
    }
}