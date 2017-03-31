using System;
using System.Xml.Serialization;

namespace Inmeta.VisualStudio.TeamExplorer
{
    [Serializable]
    public class BuildExplorerSettings
    {
        public enum QueueDefaultBuildControlValues
        {
            DontAllow = 0,
            DontAllowOnFolder = 1,
            Allow = 2
        }

        public BuildExplorerSettings()
        {
            Separator = ".";
            QueueDefaultBuildControl = QueueDefaultBuildControlValues.Allow;
        }

        [XmlElement("Separator")]
        public string Separator { get; set; }
 
        [XmlElement("QueueDefaultBuildControl")]
        public QueueDefaultBuildControlValues QueueDefaultBuildControl { get; set; }
}
}
