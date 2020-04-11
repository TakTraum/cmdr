# Development Info

Building and Development info goes here

## Quickest editing

The quickest way to edit is to change directly "Traktor Settings.tsi", and run traktor without a collection.
When you finish edits, just restart traktor (no need to import mappings, etc)

* backup collection.nml
* open Traktor
  * remove whole collection
* open cmdr
  * open TraktorSettings.tsi
  * do edits
* traktor
  * close traktor
* cmdr
  * save file with edits
* restart traktor

* repeat editing loop with cmdr always open

## Development Quick Start

The simplest way to get started is to install VS2017 and implement a small feature via "cloning".
This ensures the WPF "glue" will work immediatly.

Another simple approach is to read the check-ins of small features 
(small changes often touch a lot of files because of the WPF "glue")

## Building using VS2017

* install VS2017 community edition. Do not install VS2019! 
* install .net and c# development pack 
* get blend for windows as an individual component
* Integrate VS2017 with github: [link](https://blogs.msdn.microsoft.com/benjaminperkins/2017/04/04/setting-up-and-using-github-in-visual-studio-2017/)

## Forum Threads:

*  https://www.native-instruments.com/forum/threads/release-cmdr-controller-manager-done-right.269818/page-2
*  https://forum.djtechtools.com/showthread.php?t=91303
*  https://forum.djtechtools.com/showthread.php?t=84669
    

## Specific Versions used (as of Dec 2019):
	Microsoft Visual Studio Community 2017 
	Version 15.9.13
	VisualStudio.15.Release/15.9.13+28307.718
	Microsoft .NET Framework
	Version 4.8.03752
	Installed Version: Community

	C# Tools   2.10.0-beta2-63501-03+b9fb1610c87cccc8ceb74a770dba261a58e39c4a
	Common Azure Tools   1.10
	NuGet Package Manager   4.6.0
	ProjectServicesPackage Extension   1.0
	ResourcePackage Extension   1.0
	ResourcePackage Extension   1.0
	Visual Basic Tools   2.10.0-beta2-63501-03+b9fb1610c87cccc8ceb74a770dba261a58e39c4a
	Visual Studio Code Debug Adapter Host Package   1.0

## Visual studio tips 

Cant change code on the fly in the debugger
 - ensure code is specifically 32 bit:
   -  https://stackoverflow.com/questions/10113532/how-do-i-fix-the-visual-studio-compile-error-mismatch-between-processor-archit

RealClean extension (removes binaries as well)
  https://marketplace.visualstudio.com/items?itemName=FlorentGoetz.RealCleanExt

## git cheatsheet

This is how the filtering remote branch was merged.
  git clone "https://github.com/TomWeps/cmdr.git"
  git branch -a -vv
  git checkout remotes/origin/feature/filtering  
    


	
## WPF Filtering

pull request: https://github.com/pestrela/cmdr/commit/b7b6312bf4e9fe7649e1d8a88d6b71a910489465
original WPF example: https://www.codeproject.com/Articles/41755/Filtering-the-WPF-DataGrid-automatically-via-the-h
unchanged datagrid tutorial: https://www.wpftutorial.net/DataGrid.html 

