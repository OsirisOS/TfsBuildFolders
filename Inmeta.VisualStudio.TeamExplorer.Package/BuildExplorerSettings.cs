using System;
using System.Xml.Serialization;

namespace Inmeta.VisualStudio.TeamExplorer
{
    [Serializable]
    public class BuildExplorerSettings
    {
        public BuildExplorerSettings()
        {
            Separator = ".";
        }
        [XmlElement("Separator")]
        public string Separator { get; set; }
    }
}
