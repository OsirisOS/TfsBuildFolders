using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using EnvDTE;
using EnvDTE80;
using Fasterflect;
using Inmeta.VisualStudio.TeamExplorer.HierarchyFactory;
using Inmeta.VisualStudio.TeamExplorer.Plugin;
using Inmeta.VisualStudio.TeamExplorer.TeamExplorerNodes;
using Inmeta.VisualStudio.TeamExplorer.ToolsOptions;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamFoundation;

namespace Inmeta.VisualStudio.TeamExplorer.ExplorerNodes
{
    internal class BuildDefinitionUIHierarchy : BaseUIHierarchy, IVsSelectionEvents
    {
        private readonly Timer timer;

        private uint _monitorSelectionCockie;
        private static BuildDefinitionUIHierarchy _instance;

        public static BuildDefinitionUIHierarchy Hierarchy
        {
            get { return _instance; }
        }

        public BasicHelper GetBasicHelper
        {
            get { return this.BasicHelper; }
        }

        private readonly ITFS TFS;
        public BuildDefinitionUIHierarchy(IVsUIHierarchy parentHierarchy, uint itemId, BasicAsyncPlugin plugin, ITFS tfs)
            : base(parentHierarchy, itemId, plugin, InmetaVisualStudioTeamExplorerPackage.Instance)
        {
            TFS = tfs;
            _instance = this;
            IVsMonitorSelection monitorSelectionService = (IVsMonitorSelection)BasicHelper.GetService<SVsShellMonitorSelection>();
            monitorSelectionService.AdviseSelectionEvents(this, out _monitorSelectionCockie);
            var options = new Options(this, TFS);
            timer = new Timer(options.TimerDelayBeforeRefresh);
            timer.Elapsed += OnTimer;
            timer.Enabled = options.UseTimedRefreshAtStartup;
            string name;
            parentHierarchy.GetCanonicalName(itemId, out name);




        }



        private void OnTimer(object source, ElapsedEventArgs e)
        {
            timer.Enabled = false;
            //var clk = new Stopwatch();   // Use the clock information in Tracepoints, commented out for production code
            //clk.Start();
            int no = RefreshTree();
            //clk.Stop();
            if (no == 0)  // If we didnt find anyone, the build tree has not finished at all. It will always return at least one, the All build definitions node
                timer.Enabled = true;
        }

        public override int ExecCommand(uint itemId, ref Guid guidCmdGroup, uint nCmdId, uint nCmdExecOpt, IntPtr pvain, IntPtr p)
        {
            if (guidCmdGroup == GuidList.guidInmeta_VisualStudio_TeamExplorer_PackageCmdSet)
            {
                var node = NodeFromItemId(itemId) as ICommandableNode;
                switch (nCmdId)
                {
                    case GuidList.BtnRefresh:
                        RefreshTree();
                        break;

                    case GuidList.BtnQeueNewBuild:
                        {
                            if (node != null)
                                node.QueueNewBuild();
                        }
                        break;
                    case GuidList.BtnEditDefinition:
                        {
                            if (node != null)
                                node.OpenEditBuildDefintion();
                        }
                        break;
                    case GuidList.BtnViewBuilds:
                        {
                            if (node != null)
                                node.ViewBuilds();
                        }
                        break;
                    case GuidList.BtnOptions:
                        {
                            Options();
                        }
                        break;
                    case GuidList.BtnQueueDefaultSubBuilds:
                        {
                            if (node != null)
                                node.QueueDefaultSubFolderBuilds();
                        }
                        break;
                    case GuidList.BtnViewAllBuilds:
                        {
                            if (node != null)
                                node.ViewAllBuilds();
                        }
                        break;
                    case GuidList.BtnGotoTEBuildNode:
                        {
                            if (node != null)
                                node.GotoTeamExplorerBuildNode();
                        }
                        break;
                }

                return VSConstants.S_OK;
            }

            return base.ExecCommand(itemId, ref guidCmdGroup, nCmdId, nCmdExecOpt, pvain, p);
        }


        public void Options()
        {
            using (var options = new OptionsForm(Hierarchy, TFS))
            {
                options.ShowDialog();
            }
        }

        public override int QueryStatusCommand(uint itemId, ref Guid guidCmdGroup, uint cCmds, OLECMD[] cmds, IntPtr pCmdText)
        {
            var options = new Options(this, TFS);
            if (guidCmdGroup == GuidList.guidInmeta_VisualStudio_TeamExplorer_PackageCmdSet)
            {
                //this gory code makes the correct context commands visible depending on its position in the hierarchy (root, branch or leaf). 
                if (cCmds > 0
                    && (cmds[0].cmdID == GuidList.BtnEditDefinition ||
                    cmds[0].cmdID == GuidList.BtnViewBuilds ||
                    cmds[0].cmdID == GuidList.BtnQeueNewBuild ||
                    cmds[0].cmdID == GuidList.BtnViewAllBuilds ||
                    cmds[0].cmdID == GuidList.BtnGotoTEBuildNode ||
                    cmds[0].cmdID == GuidList.BtnQueueDefaultSubBuilds
                    ))
                {
                    var result = (int)OLECMDF.OLECMDF_SUPPORTED | (int)OLECMDF.OLECMDF_ENABLED;
                    result |= (int)OLECMDF.OLECMDF_INVISIBLE;  // Make all commands invisible

                    var node = NodeFromItemId(itemId);
                    if (node != null)
                    {
                        if (node is BuildDefinitionExplorerNode)
                        {
                            //if not children invisible = true
                            if (node.FirstChild == null)
                            {
                                if ((cmds[0].cmdID != GuidList.BtnQueueDefaultSubBuilds && cmds[0].cmdID != GuidList.BtnEditDefinition)  // Always stay off for leaf nodes, except for the case in the next two lines
                                    || (cmds[0].cmdID == GuidList.BtnQueueDefaultSubBuilds && (options.QueueDefaultBuild == BuildExplorerSettings.QueueDefaultBuildControlValues.Allow))
                                    || (cmds[0].cmdID == GuidList.BtnEditDefinition && !(node as BuildDefinitionExplorerNode).IsAllBuildDefinitionsNode)
                                    )
                                    result &= ~(int)OLECMDF.OLECMDF_INVISIBLE;
                            }
                            else // folder nodes
                                if ((cmds[0].cmdID == GuidList.BtnQueueDefaultSubBuilds && options.QueueDefaultBuild != BuildExplorerSettings.QueueDefaultBuildControlValues.DontAllow) ||
                                    cmds[0].cmdID == GuidList.BtnViewAllBuilds)
                                    result &= ~(int)OLECMDF.OLECMDF_INVISIBLE;
                        }
                        else if (node is BuildDefinitionExplorerRoot)
                        {
                            if ((cmds[0].cmdID == GuidList.BtnViewAllBuilds))
                                result &= ~(int)OLECMDF.OLECMDF_INVISIBLE;  // Turn on View All Builds
                        }
                    }

                    cmds[0].cmdf = (uint)result;
                }

                return VSConstants.S_OK;
            }

            return base.QueryStatusCommand(itemId, ref guidCmdGroup, cCmds, cmds, pCmdText);
        }

        public int RefreshTree()
        {
            if (HierarchyNode == null)
                return 0;
            // Recreate the tree by clearing the tree first
            while (HierarchyNode.FirstChild != null)
            {
                HierarchyNode.FirstChild.Remove();
            }
            // repopulate
            HierarchyNode.Expand(false);

            int no = PopulateTree(HierarchyNode);
            return no;
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

        public int PopulateTree(BaseHierarchyNode teNode)
        {
            var options = new Options(this, TFS);
            var sep = options.SeparatorToken;
            var root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root", sep, null, false);
            int noOfBuilds = 0;
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
                                               select buildHier.GetFieldValue("m_hierarchyManager").GetFieldValue("m_hierarchyNodes") as Dictionary<uint, BaseHierarchyNode>

                                               )
                {
                    foreach (var buildNodeKV in
                        builds.Reverse().Where(buildNodeKV => buildNodeKV.Value != null && !String.IsNullOrEmpty(buildNodeKV.Value.Name)))
                    {
                        bool disabled = buildNodeKV.Value.OverlayIconIndex == 6;   // Number found through investigation, we only look in the Build folder, we dont access the Build definitions, in order to keep performance up
                        BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + sep + buildNodeKV.Value.Name,
                                                                                 sep, root, disabled);
                        noOfBuilds++;

                    }
                    break;
                }

                //merge tree with UI nodes.
                if (noOfBuilds > 0)
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
            return noOfBuilds;
        }

        private void RecursiveBuildNodes(IBuildDefinitionTreeNode root, BaseHierarchyNode teNode, char sep)
        {
            //create nodes on this level and call recursive on children.
            foreach (var buildNode in root.Children.Select(node => new BuildDefinitionExplorerNode(node, sep, TFS)))
            {
                buildNode.AddChildren();
                teNode.AddChild(buildNode);
            }
            teNode.Expand(false);
        }
    }
}


