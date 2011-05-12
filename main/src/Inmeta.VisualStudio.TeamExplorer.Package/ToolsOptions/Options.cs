using System;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace Inmeta.VisualStudio.TeamExplorer.ToolsOptions
{
    public class Options
    {
        public char SeparatorToken { get { return Separator[0]; } }
        private const string SettingsFileName = "Inmeta.VisualStudio.BuildExplorer.Settings.xml";
        private const string TemporaryWorkspaceName = "InmetaVisualStudioBuildExplorerTempWorkspace";
        private const string CheckinComment = "Updated Inmeta Visual Studio Build Explorer Settings";
        private static string TfsUrl { get; set; }
        private static TfsTeamProjectCollection Tfs { get; set; }

        public Options(BaseUIHierarchy hierarchy)
        {
            Hierarchy = hierarchy;
            settings = null;
        }

        private BuildExplorerSettings settings;

        public string Separator
        {
            get 
            { 
                if (settings==null)
                    settings = GetSettings();
                return settings.Separator;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("Seperator token cannot be empty/null");
                VersionControlServer vcs = GetVersionControlServer();
                string settingsServerPath = GetSettingsFilePath(Hierarchy.ProjectName, SettingsFileName);
                string localPath;
                Workspace ws = GetWorkspaceForSettings(vcs, settingsServerPath, out localPath);

                if (!vcs.ServerItemExists(settingsServerPath, ItemType.File))
                {
                    CreateDirectoryIfNotExist(localPath);
                    SaveSettings(localPath, value);
                    ws.PendAdd(localPath);
                }
                else
                {
                    ws.Get(new GetRequest(new ItemSpec(settingsServerPath, RecursionType.None), VersionSpec.Latest), GetOptions.GetAll | GetOptions.Overwrite);
                    ws.PendEdit(localPath, RecursionType.OneLevel);
                    SaveSettings(localPath, value);
                }
                var pendingschanges = ws.GetPendingChanges(settingsServerPath);
                if (pendingschanges.Length > 0)
                {
                    ws.CheckIn(pendingschanges, CheckinComment, null, null, new PolicyOverrideInfo(CheckinComment, null));
                    if (ws.Name == TemporaryWorkspaceName)
                    {
                        ws.Delete();
                    }
                }
                //reset settings.
                settings = null;

                
                
            }
        }

        public  BaseUIHierarchy Hierarchy { get; set; }

        private static void CreateDirectoryIfNotExist(string localPath)
        {
            string folder = Path.GetDirectoryName(localPath);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        private static string GetSettingsFilePath(string teamProject, string fileName)
        {
            return String.Format("$/{0}/BuildProcessTemplates/{1}", teamProject, fileName);            
        }

        private VersionControlServer GetVersionControlServer()
        {
            var authenticatedTFS = new AuthTfsTeamProjectCollection(Hierarchy.ServerUrl);
            return authenticatedTFS.TfsVersionControlServer;
        }

        private void SaveSettings(string localPath, string separator)
        {
            settings = new BuildExplorerSettings {Separator = separator};
            Serializer.Serialize(localPath, settings);
        }

        private BuildExplorerSettings GetSettings()
        {
            VersionControlServer vcs = GetVersionControlServer();
            string settingsServerPath = GetSettingsFilePath(Hierarchy.ProjectName, SettingsFileName);
            if (!vcs.ServerItemExists(settingsServerPath, ItemType.File))
            {
                return new BuildExplorerSettings();
            }

            string localPath;
            Workspace ws = GetWorkspaceForSettings(vcs, settingsServerPath, out localPath);
            ws.Get(new GetRequest(new ItemSpec(settingsServerPath, RecursionType.None), VersionSpec.Latest), GetOptions.GetAll | GetOptions.Overwrite);
            return Serializer.DeSerialize<BuildExplorerSettings>(localPath);
        }

        private static Workspace GetWorkspaceForSettings(VersionControlServer vcs, string settingsPath, out string localPath)
        {
            var workspaces = vcs.QueryWorkspaces(null, vcs.AuthorizedUser, Environment.MachineName);

            var ws = workspaces.Where(w => w.IsServerPathMapped(settingsPath)).FirstOrDefault();
            if( ws != null)
            {
                localPath = ws.GetLocalItemForServerItem(settingsPath);
                return ws;
            }
            //Create temporary workspace
            var tmpWorkspace = workspaces.Where(w => w.Name == TemporaryWorkspaceName).FirstOrDefault();
            if (tmpWorkspace != null)
                tmpWorkspace.Delete();

            tmpWorkspace = vcs.CreateWorkspace(TemporaryWorkspaceName);
            var tmpLocalPath = Path.Combine(Path.GetTempPath(), SettingsFileName);
            if (File.Exists(tmpLocalPath))
            {
                File.SetAttributes(tmpLocalPath, FileAttributes.Normal);
                File.Delete(tmpLocalPath);
            }
            tmpWorkspace.Map(Path.GetDirectoryName(settingsPath), Path.GetDirectoryName(tmpLocalPath));
            localPath = tmpLocalPath;
            return tmpWorkspace;
        }
    }
}
