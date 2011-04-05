using System;
using System.ComponentModel.Composition;

namespace Inmeta.VisualStudio.TeamExplorer.HierarchyFactory
{
    public interface IFolderGeneratorStrategy
    {
        string[] GenerateFoldersFromName(string name, char sep);
    }

    public class FolderStratey
    {
        [Import]
        public IFolderGeneratorStrategy Splitter { get; private set; }
    }

    [Export(typeof(IFolderGeneratorStrategy))]
    public class DotSplitStrategy : IFolderGeneratorStrategy
    {
        public string[] GenerateFoldersFromName(string name, char sep)
        {
            return name.Split(new char[] { sep }, StringSplitOptions.None);
        }
    }
}
