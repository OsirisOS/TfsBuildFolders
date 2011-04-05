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

        private string GenerateToken(int num)
        {
            var result = "";
            for (var i = 0; i < num; i++)
                result += ".";
            return result;
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_EmptyRoot()
        {
            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("", '.').Validate(String.Empty);
        }
        [TestMethod]
        public void BuildDefinitionTreeNode_OnlyDots()
        {
            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree(GenerateToken(3), '.').Validate(String.Empty).ValidateNextChild(String.Empty
             , (children11) => children11.ValidateNextChild(String.Empty
                   , (children111) => children111.ValidateNextChild(String.Empty)));
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_InlineDots()
        {
            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(3) + "child1111", '.')
                .Validate("root")
                    .ValidateNextChild("child1", 
                     (children11) => children11.ValidateNextChild(String.Empty
                        , (children111) => children111.ValidateNextChild(String.Empty
                           , (children1111) => children1111.ValidateNextChild("child1111")
                           )));
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_OnlyDot()
        {
            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree(".", '.').Validate(String.Empty).ValidateNextChild(String.Empty);
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootNoDot()
        {
            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root", '.').Validate("root");
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootWithDot()
        {
            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1), '.').Validate("root");
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_EmptyRootAndChildWithDot()
        {
            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree(GenerateToken(1) + "child" + GenerateToken(1), '.')
                //root
                .Validate(String.Empty)
                    //child
                    .ValidateNextChild("child", (child1Children) => child1Children.ValidateNextChild(String.Empty));
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootAndOneChildNoDot()
        {
            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child", '.')
                //root
                .Validate("root")
                    //child
                    .ValidateNextChild("child");
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootAndOneChildWithDot()
        {
            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child" + GenerateToken(1), '.')
                //root
                .Validate("root")
                   //child
                   .ValidateNextChild("child", (child1Children) => child1Children.ValidateNextChild(String.Empty));
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootAndMutipleChildrenWithDot()
        {
            var root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "", '.')
                .Validate("root").ValidateNextChild("child1", (child1Children) => child1Children.ValidateNextChild(String.Empty)).Root();

            root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child2" + GenerateToken(1) + "", '.',root)
                .Validate("root").ValidateNextChild("child1", (child1Children) => child1Children.ValidateNextChild(String.Empty)).ValidateNextChild("child2").Root();

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child3" + GenerateToken(1) + "", '.', root)
                .Validate("root").ValidateNextChild("child1", (child1Children) => child1Children.ValidateNextChild(String.Empty)).ValidateNextChild("child2", (chil2Children) => chil2Children.ValidateNextChild(String.Empty)).ValidateNextChild("child3", (child3Children) => child3Children.ValidateNextChild(String.Empty));
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootAndMutipleChildrenNoDot()
        {
            var root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1", '.')
                .Validate("root").ValidateNextChild("child1").Root();

            root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child2", '.', root)
                .Validate("root").ValidateNextChild("child1").ValidateNextChild("child2").Root();

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child3", '.', root)
                .Validate("root").ValidateNextChild("child1").ValidateNextChild("child2").ValidateNextChild("child3");
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootAndMutipleChildrenMixedDot()
        {
            var root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "", '.')
                .Validate("root").ValidateNextChild("child1", (child1Children) => child1Children.ValidateNextChild(String.Empty)).Root();

            //no dot on this
            root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child2", '.', root)
                .Validate("root").ValidateNextChild("child1", (child1Children) => child1Children.ValidateNextChild(String.Empty)).ValidateNextChild("child2").Root();

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child3" + GenerateToken(1) + "", '.', root)
                .Validate("root").ValidateNextChild("child1", (child1Children) => child1Children.ValidateNextChild(String.Empty)).ValidateNextChild("child2").ValidateNextChild("child3", (child3Children) => child3Children.ValidateNextChild(String.Empty));
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootAndMutipleChildrenLastChildJaggedTreeMixedDot()
        {
            var root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "", '.')
                .Validate("root").ValidateNextChild("child1", (child1Children) => child1Children.ValidateNextChild(String.Empty)).Root();

            root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child2", '.', root)
                .Validate("root").ValidateNextChild("child1", (child1Children) => child1Children.ValidateNextChild(String.Empty)).ValidateNextChild("child2").Root();

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child3" + GenerateToken(1) + "child2", '.', root)
                .Validate("root").ValidateNextChild("child1", (child1Children) => child1Children.ValidateNextChild(String.Empty)).ValidateNextChild("child2").ValidateNextChild("child3", (list) => list.ValidateNextChild("child2"));
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootAndMutipleChildrenJaggedTreeMixedDot()
        {
            var root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "", '.')
                .Validate("root").ValidateNextChild("child1", (child1Children) => child1Children.ValidateNextChild(String.Empty)).Root();

            root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child2", '.', root)
                .Validate("root").ValidateNextChild("child1", (child1Children) => child1Children.ValidateNextChild(String.Empty)).ValidateNextChild("child2").Root();

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "child11", '.', root)
                .Validate("root").ValidateNextChild("child1"
                                                    , (child1Children) => child1Children.ValidateNextChild(String.Empty).ValidateNextChild("child11"))
                                 .ValidateNextChild("child2");
        }

        [TestMethod]
        public void BuildDefinitionTreeNode_RootAndLargeTreeWithMixedDot()
        {
            var root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1", '.')
                .Validate("root").ValidateNextChild("child1").Root();

            root = BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child2", '.', root)
                .Validate("root").ValidateNextChild("child1").ValidateNextChild("child2").Root();

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "child11", '.', root)
                .Validate("root").ValidateNextChild("child1",
                                                    (list) => list.ValidateNextChild("child11"))
                                 .ValidateNextChild("child2");

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "child12", '.', root)
                .Validate("root").ValidateNextChild("child1",
                                                    (list) => list.ValidateNextChild("child11").ValidateNextChild("child12"))
                                 .ValidateNextChild("child2");

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "child12" + GenerateToken(1) + "child121", '.', root)
                .Validate("root").ValidateNextChild("child1",
                                                    (child1Children) => 
                                                        child1Children.ValidateNextChild("child11")
                                                                      .ValidateNextChild("child12",
                                                                      (child12Children) => child12Children.ValidateNextChild("child121")))
                                 .ValidateNextChild("child2");

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "child12" + GenerateToken(1) + "child122", '.', root)
                .Validate("root").ValidateNextChild("child1",
                                                    (child1Children) =>
                                                        child1Children.ValidateNextChild("child11")
                                                                      .ValidateNextChild("child12",
                                                                      (child12Children) => child12Children.ValidateNextChild("child121").ValidateNextChild("child122")))
                                 .ValidateNextChild("child2");

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child2" + GenerateToken(1) + "child21" + GenerateToken(1) + "child211", '.', root)
                .Validate("root").ValidateNextChild("child1",
                                                    (child1Children) =>
                                                        child1Children.ValidateNextChild("child11")
                                                                      .ValidateNextChild("child12", (child12Children)
                                                                          => child12Children.ValidateNextChild("child121")
                                                                                            .ValidateNextChild("child122")))
                                 .ValidateNextChild("child2", 
                                                    (child2Children) => 
                                                        child2Children.ValidateNextChild("child21", (child21Children) 
                                                                          => child21Children.ValidateNextChild("child211")));

            BuildDefinitionTreeNodeFactory.CreateOrMergeIntoTree("root" + GenerateToken(1) + "child1" + GenerateToken(1) + "child13" + GenerateToken(1) + "child131", '.', root)
                .Validate("root").ValidateNextChild("child1",
                                                    (child1Children) =>
                                                        child1Children.ValidateNextChild("child11")
                                                                      .ValidateNextChild("child12", (child12Children)
                                                                          => child12Children.ValidateNextChild("child121")
                                                                                            .ValidateNextChild("child122"))
                                                                      .ValidateNextChild("child13", (child13Children)
                                                                          =>  child13Children.ValidateNextChild("child131")))
                                 .ValidateNextChild("child2",
                                                    (child2Children) =>
                                                        child2Children.ValidateNextChild("child21", (child21Children)
                                                                          => child21Children.ValidateNextChild("child211"))
                                                                                                              
                                                                          );
        }
    
    }

    public static class ValidateExtension
    { 
        private static IBuildDefinitionTreeNode _currentRoot = null;
        
        public static List<IBuildDefinitionTreeNode> Validate(this IBuildDefinitionTreeNode src, string name)
        {
            _currentRoot = src;
            Assert.AreEqual(src.Name, name, "Name mismatch");

            return new List<IBuildDefinitionTreeNode>(src.Children);

        }

        public static List<IBuildDefinitionTreeNode> ValidateNextChild(this List<IBuildDefinitionTreeNode> children, string name, Action<List<IBuildDefinitionTreeNode>> ValidateChildren)
        {
            //Validate first child's children. This will enable nested validation w/o breaking this ValidateNextChild
            ValidateChildren(new List<IBuildDefinitionTreeNode>(children[0].Children));
            return new List<IBuildDefinitionTreeNode>(children.GetRange(1, children.Count - 1)); 
        }


        public static List<IBuildDefinitionTreeNode> ValidateNextChild(this List<IBuildDefinitionTreeNode> children, string name)
        {
            Assert.AreEqual(children[0].Name, name, "Name mismatch");
            return new List<IBuildDefinitionTreeNode>(children.GetRange(1, children.Count -1));
        }

        public static IBuildDefinitionTreeNode Root(this List<IBuildDefinitionTreeNode> src)
        {
            return _currentRoot;
        }
    }
}
