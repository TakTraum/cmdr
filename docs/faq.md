# frequently asked questions

having trouble with cmdr? read this FAQ and the [User Guide](User-Guide), which gives an overview and some hints. 

if you´re still having issues please contact us via [discussions](https://cmdr.codeplex.com/discussions) or on [twitter](https://twitter.com/cmdoneright). on this page we collect solutions that might help in the first place.

### i can´t add any commands
A: check if there is a device existent in the tsi file. if you create a new tsi file you need to insert a device.

### the encoder rotation is messed up
A: check the encoder mode setting (found in the device editor, where you also assign the device target and MIDI in/out). if your controller sends high resolution encoder turns, you need to set 7Fh, and 3Fh for low res.

### can´t open file a.k.a. another word on encoders
A: make sure you have at least one encoder command with a MIDI definition (bound to Ch.xx Note xx or CC xx) so cmdr can save the encoder mode, if you plan to have encoder commands in the device. if you save encoder commands and do not have any of them with a valid MIDI binding then the file cannot be opened in cmdr (as for v0.9) --- but you could try to open it in traktor (and save back to another tsi file).

### i need to map a jog wheel
A: if you need to bind two MIDI msg to one control use the **Combo** button to add a second msg.

### what you want from my settings.tsi file?
A: since cmdr v.0.6 we´d like to read your traktor settings.tsi file. on first run, there´s a pop up window asking for the location.

be assured, there will be no change to your settings.tsi (unless you want this). so point the software to the Native Instruments folder. it´s commonly found inside your Documents folder. you can change this setting anytime using **Extras > Settings**.

after you give the path to the Native Instruments folder, you can select which settings.tsi file should be the truth. that means, with the traktor version selector you can jump around between traktor installations if you want to. 

if you decide not to point to the Native Instruments folder, you still can use the cmdr as you know it, but the three command {"FX Unit > Effect * Selector"} won´t allow for Direct Value. this is to protect your current mapping settings: if you have direct values on these commands in your mapping files and we don´t know the effects sequence of your traktor settings, we´d rather not touch them.

### i see unknown commands or conditions in my mapping
A: that´s great! please send the mapping (or the respective portion of it) to us or open an issue and include a short notice what they stand for. we´re up to integrate them into the cmdr. so be sure to always use the latest version of it. there is a list of already known "unknown commands" and helping out is appreciated: [unknown commands](https://docs.google.com/spreadsheets/d/12kR4_vYl5dBQ8jfB6dPyeW3vXtuOJJTbjGENh9colUc)

### can´t wait until the next version is released, how to run the latest source code?
A: now we´re talking. it´s quite easy, but you´ll need a C# compiler. get [MS Visual Studio](https://www.visualstudio.com/post-download-vs?sku=community&clcid=0x409) and the [latest commit of cmdr](https://cmdr.codeplex.com/SourceControl/latest) and _beware that this is not a stable release you´re downloading_. always remember this, but if you run into errors let us know.
open the cmdr.sln file with Visual Studio and hit CTRL+F5 to start cmdr without debugging. it will compile and run, if everything went well. remember, you´ve just downloaded the latest code which does not necessarily run as well as a hand-selected and tested release.

### can this editor run on Mac or Linux?
A: we don´t know. it´s written in {"C#"} and a compiler for Linux exists. so maybe.