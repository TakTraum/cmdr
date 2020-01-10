# ChangeLog

## Major milestones

* 2014: ivanz reverses engineer the TSI format using the 010 SweetScape editor
  * https://github.com/ivanz/TraktorMappingFileFormat/wiki
* 2016: csurfleet releases a first TSI editor
  * https://forum.djtechtools.com/showthread.php?t=84669&page=6
* 2016: taktraum first public relase of cmdr in codeplex: v0.6 
  * https://forum.djtechtools.com/showthread.php?t=91303
* Sep 2017: taktraum  migrates to github. Lastest version is v0.9.6 
  * https://github.com/TakTraum/cmdr/releases
* Jan 2020: pestrela makes first release after 2 years (v0.9.7) 
  * https://github.com/TakTraum/cmdr/releases
  
 
  
## Recent changes

release 0.9.8 - TBD January 2020
* New features:
  * S4Mk3, S2MK4 and S8 support  
* Changes to existing features:
  * the feature that limited the FX list to the minimum set is now OPTIONAL. This feature made very hard to merge multiple mappings with different FX lists. A future optional enhancement will force Traktors' default list of all 43 effects always.
  * can now force a save at all times, even without hanges. This is to enable the fastest way to test mappings by closing traktor, rewriting the main settings TSI, and start traktor
* Major bug fixes:
  * no more bogus duplicated row with same IDs on first copy/paste

 

release 0.9.7 - 7 January 2020
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
* Changes to existing features:
  * sorted commands by the same order as controller manager
  * removed slow pop-up "fade" animations, tri-state sorting, and inability to extend ranges using shift
  * selected notes are now a list instead of tree
  * moved codeplex links to github
  
  
  
## Old Changelog

v0.9.6 - TakTraum  Sep 9, 2017
* Fixes:
  * DryWet ranges were set to -1..1. Now set to 0..1
  * SetMasterTempo ranges were set to 0..1. Now set to 40..300
* New Features:
  * Changed column sorting to tri-state. Restore the original order by clicking the column header a third time.
  * The dialog for effect identification can now only be closed via OK button or Enter key. Added a Help link to an explanation in the Codeplex wiki.
  * Don't show decimal places in tick bar if value can be interpreted as integer 
  * Changed value sliders for float values to have a dynamic TickFrequency
  * Moved repository from Codeplex to GitHub

v0.9.5 - TakTraum  Sep 9, 2017

* Fixes:
  * Rotary Sensitivity - 'Override Factory Map' was interpreted invertedly
  * DeckSize enumeration for command 'Deck Size Selector'
  * Assignment options for commands 'Deck Size Selector' and 'Advanced Panel Toggle'
  * Midi notes menu: highest note is G9
  * Initialization of EncoderMode
  * Search could lead to application crash
* New features:
  * Option 'None' to device ports (In & Out)
  * List of recently opened files: accessible via Menu 'File', stored in application settings. The last 10 files are stored
  * Increment/decrement of generic midi bindings. Added corresponding dropdown button with options {"+"}/- (1..16 & selection count) to midi binding section of mapping details pane. Added options to increment / decrement by one to Menu 'Edit' (hotkeys Ctrl{"+"}OemPlus and Ctrl{"+"}OemMinus)
  * Apply a range of midi notes for generic midi bindings. If more than one mapping entry is selected, the first is taken as basis for subsequent increase of CC values / keys. Added corresponding option 'Apply Midi Range' to 'Advanced Options' menu.
  * Show indication during search text input if text can't be found
  * New commands and conditions for Traktor 2.11 (Step Sequencer + Ableton Link)  
  
  
  
v0.9 July 2016
* Encoder Mode:
  * [fix] Encoder modes are now processed and stored correctly.
  * Encoder mode is now a devicewide setting, thus moved to device editor pane (only shown for generic midi devices).
  * Copy&paste / drag&drop for mappings take encoder modes into account: they are adapted to the target device.

* Midi Binding:
  * In and Out commands can be mapped at the same time if there is an intersection of valid definitions (e.g. it's not possible to map a 'Level Meter' note to an In command). For generic midi devices this is always possible. For proprietary devices it depends on the note definitions.
  * Allow changing channel for various selected notes
  * Allow changing note for various selected channels
  * Added context menu items for selected channels and notes (as with conditions)
  * [fix] Issue 627: Exception on MIDI learn
  * [fix] Simultaneous MIDI learn of multiple mappings

* Devices:
  * Added drag&drop between devices. Hovering over a device while dragging selects the device. If there is no device in the target file, an appropriate device is added automatically.
  * Added drag&drop for devices: one device at a time within file or between files.
  * [fix] Initial mapping selection when switching the selected device.
  * [fix] Keep selection of mappings when switching the selected device.
  * Added the number of devices to Device list

* Advanced Options Menu:
  * Added to Mapping Editor
  * Added advanced option "Change Assignment", which changes the selected mappings' assignment as well as their condition assignments, if all selected mappings have the same assignment beforehand.

* Miscellaneous:
  * Added option to add corresponding Out command to selected In command. Assignment, Conditions and Binding are taken from selected In command.
  * Added metadata to devices. Will be used for assigning names to condition sets in the future.
  * Renamed "Details" pane to "Mapping Editor" (naming consistency)
  * [fix] Issue 631: Opening files with empty FX selection list
  * [fix] Sorting of context menu items
  * [fix] Inline editing of comments

  
v0.8 May 2016
* Creation of mappings for proprietary devices including "Notes". This feature is based on the folder of default mappings (see settings). Entries of default mappings can be included. This also enables the usage of templates.
* New mapping list control:
Drag & drop within a list and between lists. Hold ctrl to copy.
Stats (selected/total mappings)
Trimmed texts have a ToolTip containing the full text
Selection of multiple items, e.g. when pasting or moving to another list
* Adding and pasting items at the current position (not the end of the list anymore)
* Complete rework of conditions editor:
New option "Selected Conditions"
Changed handling of unknown conditions on UI
* Changed default file name to "Untitled"
* Menus for adding commands are sorted alphabetically (folders on top)
* some minor fixes, refactoring and cosmetics 
  
v0.7 April 2016
* cmdr is now the first tool that enables you to copy and paste mappings directly from and to your Traktor Settings
* Completely reworked the handling of effect selector commands and their related TSI files, even those that do not specify an effect list/snapshots
* Added status bar, indicating long running activities. Long running activities made asynchronous
* Preparation for creating new TSI files for proprietary devices
* Changed application settings and added explanations
* New (partly undocumented) commands
* New conditions
* Encoder control now initialized with Interaction Mode Relative
* some fixes, refactoring and cosmetics
  
  
  
v0.6 - TakTraum April 2016

* Drag & drop of files and folders onto application window to open TSI files. Dropping folders opens contained files recursively.
* Read list and order of effects from "Traktor Settings.tsi" dynamically (needed for effect selector commands, as not the effect itself, but its index in this list is stored in the TSI file).
* Conditions:
  * Selecting mappings and changing conditions should be a bit faster now.
  * Setting new conditions was missing initialization, so that wrong values occurred sometimes.
* Update comments while typing. No need to move focus in order to apply changes.
* Moved Override Factory Map to "Mapped to" section
* New style for numeric up/down controls in controller settings.
* Added application settings, editor window and corresponding menu entry. Settings:
  * default workspace (where you save your mappings)
  * NI folder (folder containing Traktor versions)
  * Traktor version and option to manually override it (version is written into TSI files)
  * Traktor folder is derived from NI folder and version, and is needed for loading "Traktor Settings.tsi"
* some refactoring and cosmetics
  
  
## 2020 WishList

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
 
  
  
