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
            return AssociatedNode(node.CanonicalName.Split('/')[0], node);
       }

        internal  static BaseHierarchyNode AssociatedNode(string nodeName, BaseHierarchyNode hierarchyNode)
        {
            lock (_syncLock)
            {
                try
                {
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
                                                      from build in builds.Where(build => nodeName == build.Value.Name)
                                                      select build)
                        //this is the origional builddefinition node.
                        return build.Value;
                }
                catch
                {
                }
                BuildDefinitionUIHierarchy.Hierarchy.RefreshTree();
                throw new ArgumentException("Build definition with path name '" + nodeName + "' does not exist!" +
                                            Environment.NewLine + "Inmeta Build Explorer has been refreshed.");
            }
        }

    }
}
