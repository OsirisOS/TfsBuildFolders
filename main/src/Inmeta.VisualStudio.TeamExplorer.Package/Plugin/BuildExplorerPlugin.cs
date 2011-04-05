using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Fasterflect;
using Inmeta.VisualStudio.TeamExplorer.ExplorerNodes;
using Inmeta.VisualStudio.TeamExplorer.Resources;
using Inmeta.VisualStudio.TeamExplorer.ToolsOptions;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Shell.Interop;

namespace Inmeta.VisualStudio.TeamExplorer.Plugin
{
    /// <summary>
    /// Here the explorer is created in the Team explorer
    /// This is the entry point for the plugin.
    /// </summary>
    [Guid("97CE787C-DE2D-4b5c-AF6D-79E254D83111")]
    public class BuildExplorerPlugin : BasicAsyncPlugin
    {
        private static IntPtr _sIconHandle;
        private static NodeIcons _sIcons;
        private static bool _sIconsLoaded;
        private static readonly object _lock = new object();
        private static IntPtr _sOpenIconHandle;
 
        public BuildExplorerPlugin()
            : base(InmetaVisualStudioTeamExplorerPackage.Instance)
        {
        }

        private static void EnsureIconsLoaded()
        {
            if (!_sIconsLoaded)
            {
                lock (_lock)
                {
                    if (!_sIconsLoaded)
                    {
                        _sIcons = new NodeIcons(Resource.TeamExplorerIcons);
                        var image = _sIcons.IconStrip.Images[3];
                        _sIconHandle = ((Bitmap)image).GetHicon();
                        var image2 = _sIcons.IconStrip.Images[4];
                        _sOpenIconHandle = ((Bitmap)image2).GetHicon();
                        _sIconsLoaded = true;
                    }
                }
            }
        }

        public override IntPtr IconHandle
        {
            get
            {
                EnsureIconsLoaded();
                return _sIconHandle;
            }
        }

        public static ImageList Icons
        {
            get
            {
                EnsureIconsLoaded();
                return _sIcons.IconStrip;
            }
        }

        public override IntPtr OpenFolderIconHandle
        {
            get
            {
                EnsureIconsLoaded();
                return _sOpenIconHandle;
            }
        }
 
        public override String Name
        {
            get { return "Inmeta Builds Explorer"; }
        }
        
        public override int DisplayPriority
        {
            get
            {
                // After team explorer build, but before any installed power tools
                // power tools start at 450
                return 420;
            }
        }

        protected override BaseUIHierarchy GetNewUIHierarchy(IVsUIHierarchy parentHierarchy, uint itemId)
        {
            return new BuildDefinitionUIHierarchy(parentHierarchy, itemId, this);
        }

        protected override BaseHierarchyNode CreateNewTree(BaseUIHierarchy hierarchy)
        {
            var root = new BuildDefinitionExplorerRoot("Inmeta Builds Explorer");
            if (hierarchy.HierarchyNode == null)
                hierarchy.AddTreeToHierarchy(root, true);

            return root;
        }

        
    }
}
