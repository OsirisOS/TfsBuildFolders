using System;
using System.Collections.Generic;
using System.Linq;
using Fasterflect;
using Inmeta.VisualStudio.TeamExplorer.ToolsOptions;
using Microsoft.TeamFoundation.Common;

namespace Inmeta.VisualStudio.TeamExplorer.ExplorerNodes
{
    internal static class FindAssociated
    {
        private static readonly object _syncLock = new object();

        internal static BaseHierarchyNode AssociatedNode(BaseHierarchyNode node, char sep)
        {
            //split at leaf level
            return AssociatedNode(node.CanonicalName.Split('/')[0], node, sep);
        }
        
        private static bool Compare(string a, string b)
        {
            var result = string.Compare(a, b, true);
            return result == 0;
        }

        internal static BaseHierarchyNode AssociatedNode(string nodeName, BaseHierarchyNode hierarchyNode, char sep)
        {
            var folderChars = new[] { '$', sep };
            lock (_syncLock)
            {
                try
                {
                    string nodeNameToFind = nodeName.TrimEnd(folderChars);
                    var nodes =
                        hierarchyNode.ParentHierarchy.ParentHierarchy.GetFieldValue("m_hierarchyManager").GetFieldValue(
                            "m_hierarchyNodes") as Dictionary<uint, BaseHierarchyNode>;

                    foreach (var build in from baseHierarchyNode in nodes
                                          where baseHierarchyNode.Value.CanonicalName.EndsWith("/Builds")
                                          select baseHierarchyNode.Value
                                              into node
                                              select node.NestedHierarchy as BaseUIHierarchy
                                                  into buildHier
                                                  select
                                                      buildHier.GetFieldValue("m_hierarchyManager").GetFieldValue(
                                                          "m_hierarchyNodes") as Dictionary<uint, BaseHierarchyNode>
                                                      into builds
                                                      from build in builds.Where(build => Compare(nodeNameToFind,build.Value.Name))
                                                      select build)
                        //this is the origional builddefinition node.
                        return build.Value;
                }
                catch
                {
                }
                // BuildDefinitionUIHierarchy.Hierarchy.RefreshTree();
                throw new ArgumentException("Build definition with path name '" + nodeName + "' does not exist!");
            }
        }


        /// <summary>
        /// Finds multiple matching build nodes, based on a partly name, like the name of the folder.
        /// </summary>
        /// <param name="canonicalnodeName"></param>
        /// <param name="hierarchyNode"></param>
        /// <param name="sep"></param>
        /// <returns></returns>
        internal static List<BaseHierarchyNode> AssociatedNodes(string canonicalnodeName, BaseHierarchyNode hierarchyNode, char sep)
        {
            var folderChars = new[] { '$', sep };
            string nodeName = canonicalnodeName.Split('/')[0];
            lock (_syncLock)
            {
                string nodeNameToFind = nodeName.TrimEnd(folderChars);
                var nodes =
                    hierarchyNode.ParentHierarchy.ParentHierarchy.GetFieldValue("m_hierarchyManager").GetFieldValue(
                        "m_hierarchyNodes") as Dictionary<uint, BaseHierarchyNode>;

                var nodeList = new List<BaseHierarchyNode>();
                foreach (var node in nodes)
                {
                    if (node.Value.CanonicalName.EndsWith("/Builds"))
                    {
                        var nodeNH = node.Value.NestedHierarchy as BaseUIHierarchy;
                        var buildHier =
                            nodeNH.GetFieldValue("m_hierarchyManager").GetFieldValue("m_hierarchyNodes") as
                            Dictionary<uint, BaseHierarchyNode>;
                        foreach (var build in buildHier)
                        {
                            if (build.Value != null && build.Value.Name != null && build.Value.Name.Contains(nodeNameToFind))
                                nodeList.Add(build.Value);
                        }
                    }


                }
                return nodeList;
            }
        }

    }
}
