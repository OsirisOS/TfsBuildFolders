using System;
using System.Collections.Generic;
using System.Linq;
using Fasterflect;
using Inmeta.VisualStudio.TeamExplorer.HierarchyFactory;
using Inmeta.VisualStudio.TeamExplorer.Plugin;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;

namespace Inmeta.VisualStudio.TeamExplorer.ExplorerNodes
{
    internal class BuildDefinitionUIHierarchy : BaseUIHierarchy, IVsSelectionEvents
    {
        private  uint _monitorSelectionCockie;
        private static BuildDefinitionUIHierarchy _instance;

        public static BuildDefinitionUIHierarchy Hierarchy
        {
            get { return _instance; }
        }

        public BasicHelper GetBasicHelper
        {
            get { return this.BasicHelper; }
        }

        public BuildDefinitionUIHierarchy(IVsUIHierarchy parentHierarchy, uint itemId, BasicAsyncPlugin plugin)
            : base(parentHierarchy, itemId, plugin, InmetaVisualStudioTeamExplorerPackage.Instance)
        {
            _instance = this;
            IVsMonitorSelection monitorSelectionService = (IVsMonitorSelection)BasicHelper.GetService<SVsShellMonitorSelection>();
            monitorSelectionService.AdviseSelectionEvents(this, out _monitorSelectionCockie);
        }

        public override int ExecCommand(uint itemId, ref Guid guidCmdGroup, uint nCmdId, uint nCmdExecOpt, IntPtr pvain, IntPtr p)
        {
            if (guidCmdGroup == GuidList.guidInmeta_VisualStudio_TeamExplorer_PackageCmdSet)
            {
                switch (nCmdId)
                {
                    case GuidList.BtnRefresh:
                        RefreshTree();
                        break;

                    case GuidList.BtnQeueNewBuild:
                        {
                            var node = this.NodeFromItemId(itemId);
                            if (node is ICommandableNode)
                                (node as ICommandableNode).QueueNewBuild();
                        }
                        break;
                    case GuidList.BtnEditDefinition:
                        {
                            var node = this.NodeFromItemId(itemId);
                            if (node is ICommandableNode)
                                (node as ICommandableNode).OpenEditBuildDefintion();
                        }
                        break;
                    case GuidList.BtnViewBuilds:
                        {
                            var node = this.NodeFromItemId(itemId);
                            if (node is ICommandableNode)
                                (node as ICommandableNode).ViewBuilds();

                        }
                        break;
                    case GuidList.BtnOptions:
                        {
                            Options();
                            /*var node = this.NodeFromItemId(itemId);
                            if (node is ICommandableNode)
                                (node as ICommandableNode).Options(); */
                            
                        }
                        break;
                }

                return VSConstants.S_OK;
            }

            return base.ExecCommand(itemId, ref guidCmdGroup, nCmdId, nCmdExecOpt, pvain, p);
        }


        public void Options()
        {
            using (var options = new ToolsOptions.OptionsForm(Hierarchy))
            {
                options.ShowDialog();
            }
        }

        public override int QueryStatusCommand(uint itemId, ref Guid guidCmdGroup, uint cCmds, OLECMD[] cmds, IntPtr pCmdText)
        {
            if (guidCmdGroup == GuidList.guidInmeta_VisualStudio_TeamExplorer_PackageCmdSet)
            {
                //this gory code makes the correct context commands visible depending on its position in the hierarchy (root, branch or leaf). 
                if (cCmds > 0
                    && (cmds[0].cmdID == GuidList.BtnEditDefinition || cmds[0].cmdID == GuidList.BtnViewBuilds || cmds[0].cmdID == GuidList.BtnQeueNewBuild ))
                {
                    var result = (int)OLECMDF.OLECMDF_SUPPORTED | (int)OLECMDF.OLECMDF_ENABLED;
                    result |= (int)OLECMDF.OLECMDF_INVISIBLE;

                    var node = NodeFromItemId(itemId);
                    if (node != null && node is BuildDefinitionExplorerNode)
                    {
                        //if not children invisible = true
                        if (node.FirstChild == null)
                            result &= ~(int)OLECMDF.OLECMDF_INVISIBLE;
                    }

                    cmds[0].cmdf =(uint)result;
                }

                return VSConstants.S_OK;
            }

            return base.QueryStatusCommand(itemId, ref guidCmdGroup, cCmds, cmds, pCmdText);
        }

        public void RefreshTree()
        {
            // Recreate the tree by clearing the tree first
            while (HierarchyNode.FirstChild != null)
            {
                HierarchyNode.FirstChild.Remove();
            }
            // repopulate
            HierarchyNode.Expand(false);

            PopulateTree(HierarchyNode);
        }
  
        public int OnSelectionChanged(IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMISOld, ISelectionContainer pSCOld, IVsHierarchy pHierNew, uint itemidNew, IVsMultiItemSelect pMISNew, ISelectionContainer pSCNew)
        {
            if (pHierNew == this)
            {
                if (HierarchyNode.FirstChild == null)
                {
                    RefreshTree();
                }
                return VSConstants.S_OK;
            }
          
            return VSConstants.S_OK;
        }

        public int OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
        {
            return VSConstants.S_OK;
        }

        public int OnCmdUIContextChanged(uint dwCmdUICookie, int fActive)
        {
            return VSConstants.S_OK;
        }

        public void PopulateTree(BaseHierarchyNode teNode)
        {
            var options = new ToolsOptions.Options(this);
            var sep = options.SeparatorToken;
            var root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root", sep);

            try
            {
                //This code uses reflection since this is the only way to get hold of the associated build nodes.
                //The code goes up the hierarchy and finds the 'Builds' plugin and then get hold of its build nodes. 
                var nodes = teNode.ParentHierarchy.ParentHierarchy
                    .GetFieldValue("m_hierarchyManager")
                        .GetFieldValue("m_hierarchyNodes") as Dictionary<uint, BaseHierarchyNode>;


                foreach (var builds in from baseHierarchyNode in nodes
                                       where baseHierarchyNode.Value.CanonicalName.EndsWith("/Builds")
                                       select baseHierarchyNode.Value
                                           into node
                                           select node.NestedHierarchy as BaseUIHierarchy
                                               into buildHier
                                               select buildHier.GetFieldValue("m_hierarchyManager").GetFieldValue("m_hierarchyNodes") as Dictionary<uint, BaseHierarchyNode>)
                {
                    foreach (var buildNodeKV in
                        builds.Where(buildNodeKV => buildNodeKV.Value != null && !String.IsNullOrEmpty(buildNodeKV.Value.Name) && buildNodeKV.Value.Name != "All Build Definitions"))
                    {
                        BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + sep + buildNodeKV.Value.Name, sep, root);
                    }
                    break;
                }

                //merge tree with UI nodes.
                RecursiveBuildNodes(root, teNode, sep);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
                try
                {
                    //clean up
                    while (HierarchyNode.FirstChild != null)
                    {
                        HierarchyNode.FirstChild.Remove();
                    }
                }
                catch
                {
                    //ignore something is very wrong.    
                }

                throw;
            }
        }

        private void RecursiveBuildNodes(IBuildDefinitionTreeNode root, BaseHierarchyNode teNode, char sep)
        {
            //create nodes on this level and call recursive on children.
            foreach (var node in root.Children)
            {
                var buildNode = new BuildDefinitionExplorerNode(node, sep);
                teNode.AddChild(buildNode);
                teNode.Expand(false);
                //do not create children.
                RecursiveBuildNodes(node, buildNode, sep);

            }
        }
    

    }
}


