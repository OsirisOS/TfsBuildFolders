namespace Inmeta.VisualStudio.TeamExplorer.ExplorerNodes
{
    /// <summary>
    /// This interface represents the aggregated set of functions available in the Inmeta Node Hierarchy.
    /// </summary>
    internal interface ICommandableNode
    {
        void DoDefaultAction();
        void OpenEditBuildDefintion();
        void QueueNewBuild();
        void ViewBuilds();
        void ViewAllBuilds();
        void QueueDefaultSubFolderBuilds();
        void GotoTeamExplorerBuildNode();

        void Options();
        bool IsDisabled { get; }
    }
}
