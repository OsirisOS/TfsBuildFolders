using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Inmeta.VisualStudio.TeamExplorer.Plugin;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.TeamFoundation;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

namespace Inmeta.VisualStudio.TeamExplorer
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\10.0")]
    [ProvideLoadKey("Professional", "1.0", "Inmeta Build Explorer", "www.inmeta.com", 101)]
    [ProvideService(typeof(BuildExplorerPlugin))]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0")]
    [Guid(GuidList.guidInmeta_VisualStudio_TeamExplorer_PackagePkgString)]
    [PluginRegistration(Catalogs.TeamProject, "Inmeta Build explorer", typeof(BuildExplorerPlugin))]
    [ProvideMenuResource(1000, 1)]
    public sealed class InmetaVisualStudioTeamExplorerPackage : PluginHostPackage, IVsInstalledProduct
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public InmetaVisualStudioTeamExplorerPackage()
        {
            _instance = this;
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this));
        }


        private static InmetaVisualStudioTeamExplorerPackage _instance;
        public static InmetaVisualStudioTeamExplorerPackage Instance
        {
            get { return _instance; }
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this));
            base.Initialize();

        }
        #endregion

        public int IdBmpSplash(out uint pIdBmp)
        {
            pIdBmp = 500;
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int IdIcoLogoForAboutbox(out uint pIdIco)
        {
            pIdIco = 600;
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OfficialName(out string pbstrName)
        {
            pbstrName = "Inmeta Build Explorer";
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int ProductDetails(out string pbstrProductDetails)
        {
            pbstrProductDetails = "Provides a Team Explorer plugin to visualize build definitions in a hierarchy where a given separator token indicates a sub level.";
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int ProductID(out string pbstrPID)
        {
            pbstrPID = "1.0";
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        protected override object OnCreateService(IServiceContainer container, Type serviceType)
        {
            if (serviceType == typeof(BuildExplorerPlugin))
            {
                return new BuildExplorerPlugin();
            }
            throw new ArgumentException(serviceType.ToString());
              
        }
    }
}
