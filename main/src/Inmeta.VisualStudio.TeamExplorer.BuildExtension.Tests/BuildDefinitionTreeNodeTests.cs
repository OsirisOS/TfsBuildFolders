using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Inmeta.VisualStudio.TeamExplorer.HierarchyFactory;
using Inmeta.VisualStudio.TeamExplorer.ToolsOptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inmeta.VisualStudio.TeamExplorer.BuildExtension.Tests
{
    [TestClass]
    public class BuildDefinitionTreeNodeTests
    {

        private const char CharSeparator = '.'; 

        private static string GenerateToken(int num)
        {
            var result = "";
            for (var i = 0; i < num; i++)
                result += CharSeparator;
            return result;
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_EmptyRoot()
        {
            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("", CharSeparator).ValidateAsRoot(String.Empty).VerifyAllChecked();
        }
        [TestMethod]
        public void BuildDefinitionTreeNode_OnlyDots()
        {
            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree(GenerateToken(3), CharSeparator).ValidateAsRoot(String.Empty).ValidateNextChildAsFolder(String.Empty
             , (children11) => children11.ValidateNextChildAsFolder(String.Empty
                   , (children111) => children111.ValidateNextChildAsLeaf(String.Empty))).VerifyAllChecked();
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_InlineDots()
        {
            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(3) + "child1111", CharSeparator)
                .ValidateAsRoot("root")
                    .ValidateNextChildAsFolder("child1", 
                     (children11) => children11.ValidateNextChildAsFolder(String.Empty
                        , (children111) => children111.ValidateNextChildAsFolder(String.Empty
                           , (children1111) => children1111.ValidateNextChildAsLeaf("child1111")
                           ))).VerifyAllChecked();
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_OnlyDot()
        {
            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree(".", CharSeparator).ValidateAsRoot(String.Empty).ValidateNextChildAsLeaf(String.Empty).VerifyAllChecked(); 
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootNoDot()
        {
            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root", CharSeparator).ValidateAsRoot("root").VerifyAllChecked(); 
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootWithDot()
        {
            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1), CharSeparator).ValidateAsRoot("root").ValidateNextChildAsLeaf(String.Empty).VerifyAllChecked();
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_EmptyRootAndChildWithDot()
        {
            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree(GenerateToken(1) + "child" + GenerateToken(1), CharSeparator)
                //root
                .ValidateAsRoot(String.Empty)
                    //child
                    .ValidateNextChildAsFolder("child", (child1Children) => child1Children.ValidateNextChildAsLeaf(String.Empty)).VerifyAllChecked();
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootAndOneChildNoDot()
        {
            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child", CharSeparator)
                //root
                .ValidateAsRoot("root")
                    //child
                    .ValidateNextChildAsLeaf("child").VerifyAllChecked();
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootAndOneChildWithDot()
        {
            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child" + GenerateToken(1), CharSeparator)
                //root
                .ValidateAsRoot("root")
                   //child
                   .ValidateNextChildAsFolder("child", (child1Children) => child1Children.ValidateNextChildAsLeaf(String.Empty)).VerifyAllChecked();
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootAndMutipleChildrenWithDot()
        {
            var root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "", CharSeparator)
                .ValidateAsRoot("root")
                    .ValidateNextChildAsFolder("child1", (child1Children) 
                        => child1Children.ValidateNextChildAsLeaf(String.Empty))
                .Root().VerifyAllChecked();

            root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child2" + GenerateToken(1) + "", CharSeparator,root)
                .ValidateAsRoot("root")
                    .ValidateNextChildAsFolder("child1", (child1Children)
                        => child1Children.ValidateNextChildAsLeaf(String.Empty))
                    .ValidateNextChildAsFolder("child2", (chil2Children)=> 
                        chil2Children.ValidateNextChildAsLeaf(String.Empty)).VerifyAllChecked();

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child3" + GenerateToken(1) + "", CharSeparator, root)
                .ValidateAsRoot("root").ValidateNextChildAsFolder("child1", (child1Children) 
                    => child1Children.ValidateNextChildAsLeaf(String.Empty)).ValidateNextChildAsFolder("child2", (chil2Children) 
                        => chil2Children.ValidateNextChildAsLeaf(String.Empty)).ValidateNextChildAsFolder("child3", (child3Children) 
                            => child3Children.ValidateNextChildAsLeaf(String.Empty)).VerifyAllChecked();
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootAndMutipleChildrenNoDot()
        {
            var root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1", CharSeparator)
                .ValidateAsRoot("root").ValidateNextChildAsLeaf("child1").Root().VerifyAllChecked();

            root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child2", CharSeparator, root)
                .ValidateAsRoot("root").ValidateNextChildAsLeaf("child1").ValidateNextChildAsLeaf("child2").Root().VerifyAllChecked();

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child3", CharSeparator, root)
                .ValidateAsRoot("root").ValidateNextChildAsLeaf("child1").ValidateNextChildAsLeaf("child2").ValidateNextChildAsLeaf("child3").VerifyAllChecked();
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootAndMutipleChildrenMixedDot()
        {
            var root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "", CharSeparator)
                .ValidateAsRoot("root").ValidateNextChildAsFolder("child1", (child1Children) => child1Children.ValidateNextChildAsLeaf(String.Empty)).Root().VerifyAllChecked();

            //no dot on this
            root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child2", CharSeparator, root)
                .ValidateAsRoot("root").ValidateNextChildAsFolder("child1", 
                (child1Children) => child1Children.ValidateNextChildAsLeaf(String.Empty)).ValidateNextChildAsLeaf("child2").Root().VerifyAllChecked();

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child3" + GenerateToken(1) + "", CharSeparator, root)
                .ValidateAsRoot("root").ValidateNextChildAsFolder("child1", 
                (child1Children) => 
                        child1Children.ValidateNextChildAsLeaf(String.Empty)).ValidateNextChildAsLeaf("child2")
                            .ValidateNextChildAsFolder("child3", (child3Children) => child3Children.ValidateNextChildAsLeaf(String.Empty)).VerifyAllChecked();
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootAndMutipleChildrenLastChildJaggedTreeMixedDot()
        {
            var root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "", CharSeparator)
                .ValidateAsRoot("root").ValidateNextChildAsFolder("child1", (child1Children) => child1Children.ValidateNextChildAsLeaf(String.Empty)).Root().VerifyAllChecked();

            root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child2", CharSeparator, root)
                .ValidateAsRoot("root").ValidateNextChildAsFolder("child1", (child1Children) => child1Children.ValidateNextChildAsLeaf(String.Empty)).ValidateNextChildAsLeaf("child2").Root().VerifyAllChecked();

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child3" + GenerateToken(1) + "child2", CharSeparator, root)
                .ValidateAsRoot("root").ValidateNextChildAsFolder("child1", (child1Children) => child1Children.ValidateNextChildAsLeaf(String.Empty)).ValidateNextChildAsLeaf("child2").ValidateNextChildAsFolder("child3", (list) => list.ValidateNextChildAsLeaf("child2")).VerifyAllChecked();
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootAndMutipleChildrenJaggedTreeMixedDot()
        {
            var root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "", CharSeparator)
                .ValidateAsRoot("root")
                    .ValidateNextChildAsFolder("child1", (child1Children) 
                        => child1Children.ValidateNextChildAsLeaf(String.Empty)).Root().VerifyAllChecked();

            root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child2", CharSeparator, root)
                .ValidateAsRoot("root")
                    .ValidateNextChildAsFolder("child1", (child1Children)
                        => child1Children.ValidateNextChildAsLeaf(String.Empty))
                            .ValidateNextChildAsLeaf("child2")
                    .Root().VerifyAllChecked();

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "child11", CharSeparator, root)
                .ValidateAsRoot("root").ValidateNextChildAsFolder("child1"
                                                    , (child1Children) => 
                                                        child1Children
                                                            .ValidateNextChildAsLeaf(String.Empty)
                                                            .ValidateNextChildAsLeaf("child11"))
                                                    .ValidateNextChildAsLeaf("child2").VerifyAllChecked();
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootAndLargeTreeWithMixedDot()
        {
            var root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1", CharSeparator)
                .ValidateAsRoot("root").ValidateNextChildAsLeaf("child1").Root().VerifyAllChecked();

            root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child2", CharSeparator, root)
                .ValidateAsRoot("root").ValidateNextChildAsLeaf("child1").ValidateNextChildAsLeaf("child2").Root().VerifyAllChecked();

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "child11", CharSeparator, root)
                .ValidateAsRoot("root")
                                    .ValidateNextChildAsLeaf("child1")
                                    .ValidateNextChildAsLeaf("child2")
                                    .ValidateNextChildAsFolder("child1",
                                                    (list) => list.ValidateNextChildAsLeaf("child11"))
                                    .VerifyAllChecked();

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "child12", CharSeparator, root)
                .ValidateAsRoot("root")
                                    .ValidateNextChildAsLeaf("child1")
                                    .ValidateNextChildAsLeaf("child2")
                                    .ValidateNextChildAsFolder("child1",
                                                    (list) => list.ValidateNextChildAsLeaf("child11").ValidateNextChildAsLeaf("child12"))
                                    .VerifyAllChecked();

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "child12" + GenerateToken(1) + "child121", CharSeparator, root)
                .ValidateAsRoot("root")
                                    .ValidateNextChildAsLeaf("child1")
                                    .ValidateNextChildAsLeaf("child2")
                                    .ValidateNextChildAsFolder("child1",                    
                                                    (child1Children) => 
                                                        child1Children.ValidateNextChildAsLeaf("child11")
                                                                      .ValidateNextChildAsLeaf("child12")
                                                                      .ValidateNextChildAsFolder("child12",
                                                                      (child12Children) => child12Children.ValidateNextChildAsLeaf("child121"))
                                                                      )
                                       .VerifyAllChecked();

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "child12" + GenerateToken(1) + "child122", CharSeparator, root)
                .ValidateAsRoot("root")
                                    .ValidateNextChildAsLeaf("child1")
                                    .ValidateNextChildAsLeaf("child2")
                                    .ValidateNextChildAsFolder("child1",
                                                    (child1Children) =>
                                                        child1Children.ValidateNextChildAsLeaf("child11")
                                                                      .ValidateNextChildAsLeaf("child12")
                                                                      .ValidateNextChildAsFolder("child12",
                                                                      (child12Children) => 
                                                                          child12Children.ValidateNextChildAsLeaf("child121").ValidateNextChildAsLeaf("child122")))
                                    .VerifyAllChecked();

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child2" + GenerateToken(1) + "child21" + GenerateToken(1) + "child211", CharSeparator, root)
                .ValidateAsRoot("root")
                                    .ValidateNextChildAsLeaf("child1")
                                    .ValidateNextChildAsLeaf("child2")
                                    .ValidateNextChildAsFolder("child1",
                                                    (child1Children) =>
                                                        child1Children.ValidateNextChildAsLeaf("child11")
                                                                      .ValidateNextChildAsLeaf("child12")
                                                                      .ValidateNextChildAsFolder("child12",
                                                                      (child12Children) =>
                                                                          child12Children.ValidateNextChildAsLeaf("child121").ValidateNextChildAsLeaf("child122")))
                                 .ValidateNextChildAsFolder("child2", 
                                                    (child2Children) => 
                                                        child2Children.ValidateNextChildAsFolder("child21", (child21Children)
                                                                          => child21Children.ValidateNextChildAsLeaf("child211"))).VerifyAllChecked();

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "child13" + GenerateToken(1) + "child131", CharSeparator, root)
                .ValidateAsRoot("root")
                                    .ValidateNextChildAsLeaf("child1")
                                    .ValidateNextChildAsLeaf("child2")
                                    .ValidateNextChildAsFolder("child1",
                                                    (child1Children) =>
                                                        child1Children.ValidateNextChildAsLeaf("child11")
                                                                      .ValidateNextChildAsLeaf("child12")
                                                                      .ValidateNextChildAsFolder("child12",
                                                                      (child12Children) =>
                                                                          child12Children.ValidateNextChildAsLeaf("child121").ValidateNextChildAsLeaf("child122"))
                                                                      .ValidateNextChildAsFolder("child13", (child13Children)
                                                                          => child13Children.ValidateNextChildAsLeaf("child131")))
                                 .ValidateNextChildAsFolder("child2",
                                                    (child2Children) =>
                                                        child2Children.ValidateNextChildAsFolder("child21", (child21Children)
                                                                          => child21Children.ValidateNextChildAsLeaf("child211"))

                                                                          ).VerifyAllChecked();
        }
    
    }

    public static class ValidateExtension
    { 
        private static IBuildDefinitionTreeNode _currentRoot = null;
        private static readonly List<IBuildDefinitionTreeNode> _nodesCheked = new List<IBuildDefinitionTreeNode>();
        
        public static List<IBuildDefinitionTreeNode> ValidateAsRoot(this IBuildDefinitionTreeNode src, string name)
        {
            _currentRoot = src;
            _nodesCheked.Clear();
            Assert.IsTrue(src.IsRoot, "node is not root");
            Assert.AreEqual(src.Name, name, "Name mismatch");
            _nodesCheked.Add(src);
            return new List<IBuildDefinitionTreeNode>(src.Children);

        }

        public static List<IBuildDefinitionTreeNode> ValidateNextChildAsFolder(this List<IBuildDefinitionTreeNode> children, string name, Action<List<IBuildDefinitionTreeNode>> validateChildren)
        {
            _nodesCheked.Add(children[0]);
            Assert.AreEqual(children[0].Name, name, "Name mismatch");
            Assert.IsFalse(children[0].IsLeafNode, "Node is leaf node :" + PrintNode(children[0]));

            //Validate first child's children. This will enable nested validation w/o breaking this ValidateNextChildAsFolder
            validateChildren(new List<IBuildDefinitionTreeNode>(children[0].Children));
            return new List<IBuildDefinitionTreeNode>(children.GetRange(1, children.Count - 1)); 
        }


        public static List<IBuildDefinitionTreeNode> ValidateNextChildAsLeaf(this List<IBuildDefinitionTreeNode> children, string name)
        {
            _nodesCheked.Add(children[0]);
            Assert.AreEqual(children[0].Name, name, "Name mismatch");
            Assert.IsTrue(children[0].IsLeafNode, "Node is folder node : " + PrintNode(children[0]));
            return new List<IBuildDefinitionTreeNode>(children.GetRange(1, children.Count -1));
        }

        public static List<IBuildDefinitionTreeNode> ValidateNextChildAsFolder(this List<IBuildDefinitionTreeNode> children, string name)
        {
            _nodesCheked.Add(children[0]);
            Assert.AreEqual(children[0].Name, name, "Name mismatch");
            Assert.IsFalse(children[0].IsLeafNode, "Node is leaf node : " + PrintNode(children[0]));
            return new List<IBuildDefinitionTreeNode>(children.GetRange(1, children.Count -1));
        }

        public static IBuildDefinitionTreeNode Root(this List<IBuildDefinitionTreeNode> src)
        {
            return _currentRoot;
        }

        public static IBuildDefinitionTreeNode VerifyAllChecked(this List<IBuildDefinitionTreeNode> nodes)
        {
            return _currentRoot.VerifyAllChecked();
        }

        public static IBuildDefinitionTreeNode VerifyAllChecked(this IBuildDefinitionTreeNode src)
        {
            
            Check(_currentRoot);
            VerifyAllChecked(_currentRoot.Children);

            Assert.IsTrue(_nodesCheked.Count == 0, "No check on the following nodes:" 
                                                                        + Environment.NewLine 
            
                                                                        + _nodesCheked.Aggregate("", (outString, nextNode) => outString + Environment.NewLine + PrintNode(nextNode)));
            return _currentRoot;
        }

        private static void VerifyAllChecked(IEnumerable<IBuildDefinitionTreeNode> node)
        {
            foreach (var buildDefinitionTreeNode in node)
            {
                //recursive
                VerifyAllChecked(buildDefinitionTreeNode.Children);
                Check(buildDefinitionTreeNode);
            }
        }

        private static void Check(IBuildDefinitionTreeNode node)
        {
            Assert.IsTrue(_nodesCheked.Contains(node), PrintNode(node));
            _nodesCheked.Remove(node);
        }
        private static string PrintNode(IBuildDefinitionTreeNode node)
        {
            return "Unchecked node " + node.Name + ", isLeaf = " + node.IsLeafNode + ", IsDisabled " + node.IsDisabled + ", Path = " + node.Path;
        }
    }
}
