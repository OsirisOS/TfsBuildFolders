using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace Inmeta.VisualStudio.TeamExplorer.ToolsOptions
{
    public partial class OptionsForm : Form
    {
        private readonly ToolsOptions.Options options;
        public OptionsForm(BaseUIHierarchy hierarchy)
        {
            InitializeComponent();
            options = new Options(hierarchy);
        }

        private void Save(object sender, EventArgs e)
        {
            options.Separator = tbSeparator.Text;
            this.Close();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            this.tbSeparator.Text = options.Separator;
            this.lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
