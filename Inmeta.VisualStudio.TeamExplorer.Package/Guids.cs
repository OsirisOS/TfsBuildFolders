// Guids.cs
// MUST match guids.h
using System;
using System.ComponentModel.Design;

namespace Inmeta.VisualStudio.TeamExplorer
{
    static class GuidList
    {
        public const string guidInmeta_VisualStudio_TeamExplorer_PackagePkgString = "54a9cf52-4115-4217-9f35-9ca1fc4b7083";
        public const string guidInmeta_VisualStudio_TeamExplorer_PackageCmdSetString = "21efb7b8-ed81-4375-a3e7-37e0935abfa5";

        public static readonly Guid guidInmeta_VisualStudio_TeamExplorer_PackageCmdSet = new Guid(guidInmeta_VisualStudio_TeamExplorer_PackageCmdSetString);


        public const int InmetaBuildExplorerMenu = 0x1000;
        public const uint BtnRefresh                = 0x0101;
        public const uint BtnQeueNewBuild           = 0x0102;
        public const uint BtnEditDefinition         = 0x0103; 
        public const uint BtnViewBuilds             = 0x0104;
        public const uint BtnOptions                = 0x0105;
        public const uint BtnQueueDefaultSubBuilds  = 0x0106;
        public const uint BtnViewAllBuilds          = 0x0107;
        public const uint BtnGotoTEBuildNode        = 0x0108;
        public static readonly CommandID ContextMenuCommand = new CommandID(guidInmeta_VisualStudio_TeamExplorer_PackageCmdSet, InmetaBuildExplorerMenu);

    };
}