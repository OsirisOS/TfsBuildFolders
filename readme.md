*Introduction*
The Inmeta Build Explorer adds Folders to the Team Explorer Builds.  This is a client based solution and works with the Visual Studio 2010 Team Explorer.  

The folders are created based on separators in the Build Definition name.  The default separator is the punctuation mark  (.).   
A user can change this separator per team project, and all users of the team project will use this setting. 
See what Brian Harry says about it  [url:http://blogs.msdn.com/b/bharry/archive/2011/04/01/build-folders.aspx]

Documentation of latest release 1.1.8 is here: [url:http://geekswithblogs.net/terje/archive/2011/11/15/build-explorer-version-1.1-for-visual-studio-team-explorer-is.aspx]
Documentation of former release 1.0.1 is here : [url:http://geekswithblogs.net/terje/archive/2011/05/12/new-version-of-the-inmeta-build-folder-extension.aspx]
Original blog post is here: [url:http://geekswithblogs.net/terje/archive/2011/04/01/visual-studiondashbuild-folders-extension-for-team-explorer.aspx]

*Installation*

Download the VSIX extension from the download page here or from the Visual Studio Code Gallery [url:http://visualstudiogallery.msdn.microsoft.com/35daa606-4917-43c4-98ab-38632d9dbd45] or search for Inmeta in Tools/Extension Manager
Install it just by running the vsix file. 
(If you get any problems upgrading it, you can uninstall this extension from the Tools/Extension Manager.  

*Using it*
The Inmeta Build Explorer appears as another top node for each team project.  When you start up Visual Studio it will appear empty, but it will fill up the first time you click it.
Right click the nodes to get at the context menues.  
On all nodes the Options and Refresh appears.  Options allow you to change the separation token.
On the leaf nodes, the real build definitions, the context menu have been shaved down to those most useful, and adds the following menu items in addition to the top node menu items:  
Queue New Build....
Queue Default Build(s)
View Builds 
View All Builds
Goto Team Explorer Build Node
Edit Build Definition

(If you want more, feel free to download and modify the source)

Double clicking the nodes will collapse and expand the tree, the most natural way. 

*Setting up build definitions*
As mentioned above this works by having the names of your build definitions following a convention.
Start a build definition name using the Solution to build, then end it with the type of build.
Example:  BuildExplorer, with CI build and Production build
BuildExplorer.CI
BuildExplorer.Production
In the Inmeta Build Explorer this will appear as a top node BuildExplorer with two sub nodes CI and Production

*Credits*
The idea comes from discussions within Inmeta and DnBNor IT.  Thomas Hilde from DnBNor IT was central in this.
The work was sponsored by Inmeta [url:http://www.inmeta.com] and DnBNor IT.
Most of the base code was written by Lars Nilsson ([url:http://larzjoakimnilzzon.blogspot.com/]) , and further work by Jakob Ehn ([url:http://geekswithblogs.net/jakob]) and Terje Sandstrom ([url:http://geekswithblogs.net/terje]), all from Inmeta.  Lars has written a blog post [url:http://larzjoakimnilzzon.blogspot.com/2011/04/inmeta-build-explorer.html] detailing the challenges he faced when writing the code base. 

The great Fasterflect library is used, see [url:http://fasterflect.codeplex.com/], licensed as shown here [url:http://fasterflect.codeplex.com/license]

*Developers*
Want to contribute ?  Drop us a message and we'll add you!  

*Source Code*
Available

*Storing settings*
The settings are stored per team project in the source control, under the folder  BuildProcessTemplates. The file is named Inmeta.VisualStudio.BuildExplorer.Settings.xml. 
It will only be stored if the options are changed from the default value of a punctuation mark. 
