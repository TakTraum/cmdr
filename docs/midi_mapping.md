# MIDI mapping

the cmdr allows to learn or select the MIDI message(s). select one or more commands and bind them accordingly.

#### Select
is for folks who know the implementation of their controllers. clicking this button allows you to select from all possible MIDI messages. it can be a note or a control change.

![](midimapping_http://cmdr.acidbuddha.com/img/noteselect.PNG)

for notes, we decided to provide an octave selector. so you do not need to scroll through a plain list of over hundred notes, instead you select the note and then select the octave. the cc select lists are also split in smaller chunks.

#### Learn
activates listening for MIDI messages on the port the device is attached to. if a fader or knob is moved, or a knob is pressed on the selected controller, the message is received and listening stops.

that should be convenient enough. the learn shortcut is CTRL+L. the learn mode is only available if a MIDI controller is found in the system.

#### Combo
there are occasions where you need to bind two MIDI messages to a command. this can be done using the {"[Combo](Combo)"} button. after you learned or selected the first note/cc, click this button (it will turn yellow) and learn or select another message. this will lead to a note expression like CC.042+CC.031.

![](midimapping_http://cmdr.acidbuddha.com/img/combocc.PNG)

### channel selector
an alleviation is the channel selector which is not tied to the note/cc anymore. this is great to just move a complete mapping from channel 10 to channel 9.