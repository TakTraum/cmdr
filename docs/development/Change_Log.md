# ChangeLog

## Major milestones

* 2014: ivanz reverses engineer the TSI format using the 010 SweetScape editor
  * https://github.com/ivanz/TraktorMappingFileFormat/wiki
* 2016: csurfleet releases a first editor
  * https://forum.djtechtools.com/showthread.php?t=84669&page=6
* 2016: TakTraum first public relase of cmdr in codeplex: v0.6 
  * https://forum.djtechtools.com/showthread.php?t=91303
* Sep 2017: TakTraum  migrates to github. Lastest version is v0.9.6 
  * https://github.com/TakTraum/cmdr/releases
* Sep 2017: TakTraum  migrates to github. Lastest version is v0.9.6 
  
## Recent changes

release 0.9.7 - 7 January 2010
* New features:
  * added new commands for TP3.0 (MixerFX, Flux reverse, Reverse)  
  * added [grep-style string filtering per column](https://github.com/TakTraum/cmdr/pull/9) (Contributed by Tom Weeps).
  * added clear grid filtering
  * added mass-modifier rotation:
    * as a command or condition (keyboard shortcut: CTRL+1..6)
    * as a value or condition value (keyboard shortcut: ALT+1..6)
  * added new column to sort on Notes/CCs (exiting column sorts on Channels)
  * added shortcut to mass-change channel and pad number (besides note number)
  * added shortcut to select All/None, and bring top/bottom into view 
  * ability to mass-swap conditions
  
* Changes:
  * sorted commands by same order as controller manager
  * removed slow pop-up "fade" animations, tri-state sorting
  * selected notes are now a list instead of tree
  * moved codeplex links to github

checkin:
  updated version and about box to 0.9.7
  buildtree now sorts by default; Main IN/OUT trees is unsorted on purpose
  added selectAllToggle (changes between select all/none)
  
  
  
## 2019 WishList

* Move "signed encoder mode" from device property to per-entry
* Never rewrite FX list     https://github.com/TakTraum/cmdr/issues/6
* Ensure focus always on main grid
* Separate column condition1 and condition2 
* pop up with selected comments (similar to selected notes)
* dedicated editor for same condition, but different values (eg: is in active loop) 
* search for a command in all pages 
* arrows = move selected row into view        
* better clear grid (CTRL+Q)
  

* See also the TakTraum/cmdr issue list: https://github.com/TakTraum/cmdr/issues
* See also the old Codeplex issue list (ignore the chrome warning):  https://archive.codeplex.com/?p=cmdr  
 
  
  
