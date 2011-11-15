using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Inmeta.VisualStudio.TeamExplorer.TeamExplorerNodes;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace Inmeta.VisualStudio.TeamExplorer.ToolsOptions
{
    public partial class OptionsForm : Form
    {
        private readonly ToolsOptions.Options options;
        
        public OptionsForm(BaseUIHierarchy hierarchy, ITFS tfs)
        {
            InitializeComponent();
            options = new Options(hierarchy,tfs);
        }

        private void Save(object sender, EventArgs e)
        {
            options.Separator = tbSeparator.Text;

            options.QueueDefaultBuild = rbQDBAllow.Checked
                                            ? BuildExplorerSettings.QueueDefaultBuildControlValues.Allow
                                            : (rbQDBDontAllowFolders.Checked
                                                   ? BuildExplorerSettings.QueueDefaultBuildControlValues.
                                                         DontAllowOnFolder
                                                   : BuildExplorerSettings.QueueDefaultBuildControlValues.DontAllow);
            options.UseTimedRefreshAtStartup = cbUseTimerRefresh.Checked;
            int delay;
            if (Int32.TryParse(tbDelay.Text, out delay))
                options.TimerDelayBeforeRefresh = delay;
            options.SaveSettings();
            this.DialogResult = DialogResult.None;
            Close();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            this.tbSeparator.Text = options.Separator;
            this.rbQDBAllow.Checked = rbQDBDontAllowFolders.Checked = rbQDBDontAllow.Checked = false;
            if (options.QueueDefaultBuild == BuildExplorerSettings.QueueDefaultBuildControlValues.Allow)
            {
                rbQDBAllow.Checked = true;
            }
            else if (options.QueueDefaultBuild == BuildExplorerSettings.QueueDefaultBuildControlValues.DontAllowOnFolder)
            {
                rbQDBDontAllowFolders.Checked = true;
            }
            else if (options.QueueDefaultBuild == BuildExplorerSettings.QueueDefaultBuildControlValues.DontAllow)
            {
                rbQDBDontAllow.Checked = true;
            }
            cbUseTimerRefresh.Checked = options.UseTimedRefreshAtStartup;
            tbDelay.Text = options.TimerDelayBeforeRefresh.ToString();
            this.lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
