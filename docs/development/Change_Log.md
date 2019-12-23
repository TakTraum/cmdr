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

## Recent changes

Upcoming release 0.9.7
* New features:
  * added new commands for TP3.0 (MixerFX, Flux reverse, Reverse)  
  * added [grep-style string filtering per column](https://github.com/TakTraum/cmdr/pull/9). Contributed by Tom Weeps.
  * added ability to sort on either Notes/CCs and Channels
* New Keyboard shortcuts: 
  * alt+"+"/"-": move channel up/down
  * shift+"+"/"-": move note 8 positions up/down (matches pads)
  * ctrl+9/0: Rotate assignement
  * ctrl+Q: clear grid filtering
  * ctrl+A: select all
* Modifier renaming:
  * ctrl/shift+1/2: Inc/Dec modifier condition1 (both keys and values)
  * ctrl/shift+3/4: Inc/Dec modifier condition2 (both keys and values)
  * ctrl+5/6: Inc/Dec modifier command
* Removed:
  * removed slow pop-up "fade" animations, tri-state sorting

## 2019 WishList

* Shortcut to move modifier conditions and FX assigments around
* Never rewrite FX list     https://github.com/TakTraum/cmdr/issues/6
* Move "signed encoder mode" from device property to per-entry
* Ensure focus always on main grid
* Seperate column condition1/cond2 
    
* See also the TakTraum/cmdr issue list: https://github.com/TakTraum/cmdr/issues
* See also the old Codeplex issue list (ignore the chrome warning):  https://archive.codeplex.com/?p=cmdr  

* mass swap conditions feature  <<<
* sort commands by same order as controller manager

