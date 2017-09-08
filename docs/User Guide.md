# User Guide

assuming you already have certain familiarity with the built in Controller Manager we´ve built an editor to fix your existing mappings and create new ones with comfort. the goal was to replace the built-in manager as far as possible. so you can completely define your mapping with this cmdr and only go to the controller manager to import the prepared tsi file.

you can open multiple tsi files and have them docked to copy and paste commands between them.

below the options of the cmdr are explained. the labels are mostly kept to the original so you can use the  [help file for the original controller manager](http://www.native-instruments.com/en/support/knowledge-base/show/698/how-to-use-the-controller-manager-in-traktor/) for information on assignment and behaviour options.

## a note on first start

since version 0.6 there is an Settings dialog where you can tell cmdr where to look for your traktor settings.tsi file. this is due to the fact we want to allow you to directly map the effects list. but this list is unique to your setup. in previous versions the FX selector direct value would not necessarily match with the effect that it would load on your traktor. this is fixed by reading your effects list first!

Having your Traktor Settings connected to cmdr has the following advantages:
* if you add an effect selector command (in or out), the default fx snapshot from your Traktor Settings is added to the mapping file you are currently working on. Without the connection a default snapshot is added (all values set to 0)
* (pending feature) consolidate your effect list and snapshots with your Traktor Settings 

head over to the [faq](faq) for more information on this this change.


## Devices

the devices panel shows the units of commands that are kept together in a container. a device is shown in traktor´s controller manager´s drop down list after importing the tsi file. they show up in just the order you see in the cmdr.

obvious but worth noting: you need at least one device to create In and Out command mappings. just create one using the {"[+](+)"} to start the mapping frenzy!

### Device editor

every device can have a name, which is known to traktor users as the comment. you also can prepare the device inside cmdr to use a certain MIDI port. that saves you a few clicks in traktor after importing. the MIDI In port is also read for the MIDI learn feature.

here you also can set the encoder mode device-wide. it is important to check, because traktor won´t behave correctly on encoder turns if you did not set the enc mode according to your hardware. especially if you load foreign tsi files, remember to check this setting.

## Mapping commands

this is the main workbench. the mappings table is similar to the controller manager´s one, but has got a few tweaks.

# **Id:** these are the internal ids used to manage the command list. if you copy and paste commands around these ids will change as they get appended to the last one. if you want a clean mapping copy all your commands to an empty device.
# **I/O:** shows, if the command is an In command or an Out one.
# **Command:** the command, enhanced by information where appropriate. I.e. you see if a trigger is an decrement (--) or increment (++) one, or to what value a modifier is set by a button hold. it is of much help to us while mapping, and we believe you won´t mind
# **Assignment:** like in the controller manager, it shows to what unit inside traktor the command is attached
# **Condition:** the conditions that apply to the command. you´ll notice the conditions only have one column for readability.
# **Interaction:** what type of control element is used for this command. like the command column, addtional information is put where needed.
# **Mapped to:** shows to what MIDI note or cc the command is bound. check out [midimapping](midimapping) for more.
# **Comment:** no comment. but you can edit is inline after a double click.

Below the table you find a few buttons.

**Add In** and **Add Out** inserts commands. a tree similar to the controller manager´s will open and allow you to pick what you need. the trees are sorted a-z.

**Show Conditions** is an analyzing feature to view all given conditions inside the device. very useful when debugging or viewing foreign downloaded mappings.

**Search** finds. **important notice** the search is known to be crashing cmdr so please do not use this on unsaved tsi projects until v0.91 is up.

## Details

the details panel allows you to manipulate settings of the selected command(s). even though there´s a load of additional information in the other columns we encourage use of the comment field. that´s why its input area is resizable.

**batch editing** here we go: if you select multiple commands the details section will only show you the controls you can edit for all of them. get used to it, you´ll like it. this is true for the conditions, the settings and the MIDI binding as well. right now, this is true for a subset and may increase over time. right now we show the controls that we´re sure of.

### Advanced Options
this button allows to manipulate the selected mappings even further, but these methods are not so common (we think). right now the only method is "change assignments". more to come.

##### Change Assignments
this advanced option allows to move the mapping selection from, i.e. Deck A to Deck B including all conditions regarding the origin Deck, so you can shift the assignments from Deck to Deck and save a few clicks.

![](User Guide_http://cmdr.acidbuddha.com/img/changeassignment.PNG)

in this picture, the selected mappings would be changed to "device target" in Assignment and Condition 2 Deck Flavor selector. this works for Slots and FX as well. let us know what you think about it. the change assignment options is only enabled for selections that do not contain commands with assignments of type global.

### Conditions
every command in traktor may have up to two conditions. to add a condition hit the {"[1](1)"} or {"[2](2)"} button and select one. select _None_ to remove a condition.

after a condition is set, you still can change the unit it is attached to (like the FX Unit or the Deck) without re-selecting the condition as a whole.

**batch editing conditions:** as of cmdr 0.8 the conditions menu will show a _selected conditions_ option: in there you see all conditions that appear in the commands currently selected. this is to speed up replacing conditions in batch. if your selection does not contain any conditions this option is disabled.

### Settings

according to what traktor allows a command to be controlled by, you can select the control and interaction mode for it. this means you can prepare the whole stuff without providing the MIDI binding.

the control can be one of the options Button, Fader/Knob or Encoder. each control has its interaction methods. this is exact the same as in traktor itself.

### Mapped To

this is where the command is given a corresponding MIDI binding. there are two ways to edit the MIDI binding: **select** and **learn**. To remove a binding use the {"[Reset](Reset)"} button.

for details, see the [midimapping](midimapping) page.

### Effects

if you want to know why cmdr shows a weird dialog when opening some TSI files, see [this explanation](Effects) on how Traktor handles effect mappings and why cmdr needs your help in these cases.

# tl;dr

just open the cmdr editor and start mapping. it was designed to be understandable by everybody who spent their ten hours inside the controller manager of traktor.

workflow: select device _left_, add commands _middle_, change settings {"+"} mapping _right_ and save _CTRL+S_. 
import into traktor and tweak those knobs!
