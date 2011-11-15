using System;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Inmeta.VisualStudio.TeamExplorer.Plugin;
using Inmeta.VisualStudio.TeamExplorer.TeamExplorerNodes;
using Microsoft.TeamFoundation.Common;
using Fasterflect;

namespace Inmeta.VisualStudio.TeamExplorer.ExplorerNodes
{

    public class BuildDefinitionExplorerRoot : RootNode, ICommandableNode
    {
        private const char defaultSep = '.';
        private ITFS TFS;
        public BuildDefinitionExplorerRoot(string path, ITFS  tfs)
            : base(path)
        {
            TFS = tfs;
            Name = path;
            InitAsFolder();
        }
        
        public override ImageList Icons
        {
            get
            {
                return BuildExplorerPlugin.Icons;
            }
        }

        public override void DoDefaultAction()
        {
            FindAssociated.AssociatedNode("All Build Definitions", this, defaultSep).DoDefaultAction();  // uses default separator, doesnt matter
        }

        public void OpenEditBuildDefintion()
        {
            //do nothing
        }

        public void GotoTeamExplorerBuildNode()
        {
            //do nothing
        }

        public void QueueNewBuild()
        {
            var node = FindAssociated.AssociatedNode("All Build Definitions", this, defaultSep);
            node.ParentHierarchy.CallMethod("QueueBuild", node);
        }

        public void ViewBuilds()
        {
            //do nothing
        }

        public override bool ExpandByDefault
        {
            get
            {
                return false;
            }
        }

        public void Options()
        {
            using (var options = new ToolsOptions.OptionsForm(ParentHierarchy,TFS))
            {
                options.ShowDialog();
            }
        }

        public override string PropertiesClassName
        {
            //Name of the node to show in the properties window
            get { return "Inmeta Builds Explorer"; }
        }

        public override CommandID ContextMenu
        {
            get
            {
                return GuidList.ContextMenuCommand;
            }
        }

        public bool IsDisabled
        {
            get
            {
                return false;
            }
        }


        public void ViewAllBuilds()
        {
            var node = FindAssociated.AssociatedNode("All Build Definitions", this, defaultSep);
            node.ParentHierarchy.CallMethod("ViewBuilds", node);
        }

        public void QueueDefaultSubFolderBuilds()
        {
            // ignore here, should never be called
        }


        /// <summary>
        /// Not used
        /// </summary>
        public bool IsFolderNode
        {
            get { return false; }
        }
    }

}
