using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Inmeta.VisualStudio.TeamExplorer.HierarchyFactory
{

    public static class BuildDefinitionTreeNodeFactory
    {
        private static readonly IFolderGeneratorStrategy _splitStrategy = new DotSplitStrategy();
        

        public static IBuildDefinitionTreeNode CreateOrMergeIntoTree(string path, char sep)
        {
            return CreateOrMergeIntoTree(path, sep, null,false);
        }

        public static IBuildDefinitionTreeNode CreateOrMergeIntoTree(string path, char sep, IBuildDefinitionTreeNode root)
        {
            return CreateOrMergeIntoTree(path, sep, root, false);
        }

        private static bool Compare(string a, string b)
        {
            var result = string.Compare(a, b, true);
            return result == 0;
        }

        /// <summary>
        /// Populate a IBuildDefinitionTreeNode with a new path.
        /// </summary>
        /// <param name="path">The path to to the build definition.</param>
        /// <param name="sep">The seperator</param>
        /// <param name="root">The root if not provided it will be created.</param>
        /// <param name="disabled">True if the build is marked as disabled</param>
        /// <returns>The root</returns>
        public static IBuildDefinitionTreeNode CreateOrMergeIntoTree(string path, char sep, IBuildDefinitionTreeNode root, bool disabled)
        {
            Contract.Requires(root != null);
            IBuildDefinitionTreeNode returnRoot = null;
            //path contains the parent.Name.
            var parent = root;

            //split to get parent, child and the rest if possible we want null results (StringSplitOptions.None) since that makes testing simpler.
            var parts = _splitStrategy.GenerateFoldersFromName(path, sep);
            
            
            //special case when only adding root.
            if (parts.Length == 1)
            {
                return new BuildDefinitionRootNode(parts.Last(), sep);
            }

            //generate needed hierarchy.
            //ignore last part: is it allways treated as leaf.
            for (var i = 0; i < parts.Length - 1; i++)
            {
                var part = parts[i];
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

                    //first node with same name and which is not a leaf node
                    //this will support 
                    //Builds:
                    //    test.min
                    //    test
                    var folder = parent.Children.Where(c => Compare(part1,c.Name) && !c.IsLeafNode).FirstOrDefault();
                    if (folder == null)
                    {
                        folder = new BuildDefinitionFolderNode(part, sep);
                        ((BuildDefinitionTreeNode)parent).AddChild(folder);
                    }
                    parent = folder;
                }
            }

            //allways add last part as leaf node if not only root
            if (parts.Length > 1)
            {
                // Check to see if the last part is an existing  folder 
                var part1 = parts.Last();
                var folder = parent.Children.Where(c => Compare(part1, c.Name)).FirstOrDefault();
                
                if (folder != null)  // // Check to see if the last part is an existing  folder
                {
                    parent = folder;
                    part1 = "$";
                }
                var leaf = new BuildDefinitionLeafNode(part1, sep, disabled);
                ((BuildDefinitionTreeNode)parent).AddChild(leaf);
                
            }

            return returnRoot;
        }
    }
}
