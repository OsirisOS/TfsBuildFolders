using System;
using System.Linq;

namespace Inmeta.VisualStudio.TeamExplorer.HierarchyFactory
{

    public static class BuildDefinitionTreeNodeFactory
    {
        private static readonly IFolderGeneratorStrategy _splitStrategy = new DotSplitStrategy();
        
        /// <summary>
        /// Populate a IBuildDefinitionTreeNode with a new path.
        /// </summary>
        /// <param name="path">The path to to the build definition.</param>
        /// <param name="sep">The seperator</param>
        /// <param name="root">The root if not provided it will be created.</param>
        /// <returns>The root</returns>
        public static IBuildDefinitionTreeNode CreateOrMergeIntoTree(string path, char sep, IBuildDefinitionTreeNode root = null)
        {
            IBuildDefinitionTreeNode returnRoot = null;
            //path contains the parent.Name.
            var parent = root;

            //split to get parent, child and the rest if possible we want null results (StringSplitOptions.None) since that makes testing simpler.

            var parts = _splitStrategy.GenerateFoldersFromName(path, sep);
            foreach (var part in parts)
            {
                //if root does not exists, use first part to generate root name.
                if (returnRoot == null)
                {
                    parent = root ?? new BuildDefinitionRootNode(part, sep);
                    //root 
                    returnRoot = parent;
                }
                else
                {
                    //Find the existing child with the name if any.
                    var part1 = part;
                    var child = parent.Children.Where(c => part1 == c.Name).FirstOrDefault();
                    if (child == null)
                    {
                        child = new BuildDefinitionTreeNode(part, sep);
                        ((BuildDefinitionTreeNode) parent).AddChild(child);
                    }
                    parent = child;
                }
            }

            return returnRoot;
        }
    }
}
