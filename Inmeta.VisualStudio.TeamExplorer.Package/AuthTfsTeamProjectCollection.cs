using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inmeta.VisualStudio.TeamExplorer.TeamExplorerNodes;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace Inmeta.VisualStudio.TeamExplorer
{
    internal class AuthTfsTeamProjectCollection
    {
        private static string TfsUrl { get; set; }
        internal static TfsTeamProjectCollection Tfs { get; private set; }

        [Obsolete("Use the ctor(ITFS) instead",false)]
        public AuthTfsTeamProjectCollection(string url)
        {
            if (Tfs == null || TfsUrl != url)
            {
                TfsUrl = url;
                Tfs = new TfsTeamProjectCollection(new Uri(TfsUrl), new UICredentialsProvider());
                // Tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(TfsTeamProjectCollection.GetFullyQualifiedUriForName(Tfs
            }

            Tfs.Authenticate();

        }

        private ITFS TFS;
        public AuthTfsTeamProjectCollection(ITFS  tfs)
        {
            TFS = tfs;
            if (Tfs == null || Tfs.Name!=tfs.Context.DomainName)
            {
                Tfs =
                    TfsTeamProjectCollectionFactory.GetTeamProjectCollection(
                        TfsTeamProjectCollection.GetFullyQualifiedUriForName(TFS.Context.DomainUri));
            }

        }

        internal VersionControlServer TfsVersionControlServer
        {
            get { return (VersionControlServer) Tfs.GetService(typeof (VersionControlServer)); }
        }

        internal IBuildServer TfsBuildServer
        {
            get { return (IBuildServer)Tfs.GetService(typeof(IBuildServer));}
        }


    }
}
