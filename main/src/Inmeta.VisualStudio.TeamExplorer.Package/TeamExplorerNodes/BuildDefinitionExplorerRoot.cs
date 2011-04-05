using System;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Inmeta.VisualStudio.TeamExplorer.Plugin;
using Microsoft.TeamFoundation.Common;
using Fasterflect;

namespace Inmeta.VisualStudio.TeamExplorer.ExplorerNodes
{

    public class BuildDefinitionExplorerRoot : RootNode, ICommandableNode
    {
        public BuildDefinitionExplorerRoot(string path)
            : base(path)
        {
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
            FindAssociated.AssociatedNode("All Build Definitions", this).DoDefaultAction();
        }

        public void OpenEditBuildDefintion()
        {
            //do nothing
        }

        public void QueueNewBuild()
        {
            var node = FindAssociated.AssociatedNode("All Build Definitions", this);
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
            using (var options = new ToolsOptions.OptionsForm(ParentHierarchy))
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

    }

}
