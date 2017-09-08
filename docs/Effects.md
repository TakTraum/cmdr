# Why does cmdr ask me to identify effects?

If you open a TSI file that was exported from the Controller Manager and contains effects, cmdr has no chance to identify them on its own (see below). Instead of leaving you with the wrong effects displayed, it allows you to specify other TSI files that contain a list of Pre-Selected effects. This could be the Traktor Settings TSI specified in cmdr's settings or any other TSI file. Read on for further details and remedy.

# How does Traktor handle mappings that use effects?

When you export mappings right from the Controller Manager, Traktor doesn't save the actual effects (e.g. represented by an ID) but their position in the list of Pre-Selected Effects. Same goes for mappings that relate to layouts or other user settings. This makes it difficult to share these TSI files. You can find the list of Pre-Selected Effects in the FX Pre-Selection section of Traktor's preferences window:
![](Effects_http://blog.dubspot.com/files/2012/01/FX-Pre-Selection-Traktor.png)

**Example:** _Your mapping uses the Iceverb effect. It has the internal ID 18. With the list above, Traktor wouldn't save 18 but 2, because Iceverb is at the second position in the list. When someone else imports this TSI file from the Controller Manager, the Iceverb effect will be replaced by the effect at the second position of his own list._ 

Mappings that depend on user settings can only be exported by using the big export button in the lower right corner of the preferences window. Traktor lets you choose the preferences to export. Select all categories your mappings depend on. If they use effects, have both Controller Mappings and Effect Settings checked.
![](Effects_http://11234-presscdn-0-32.pagely.netdna-cdn.com/wp-content/uploads/2012/05/traktor25preferencessave.jpg)

Unfortunately, it is not possible to select single device mappings. All devices from the Controller Manager will be exported. The only way to export a complete mapping for a single device is to remove all other devices from the Controller Manager. Make sure to create a backup beforehand.

See [this DJ TechTools article](http://djtechtools.com/2015/06/14/troubleshooting-midi-maps-in-traktor/) for more details on importing/exporting mappings in Traktor.

# The Golden Path

cmdr helps you to export and import mappings without these limitations. It makes sure that all created files are complete and ready to share. _At the moment effects are the only user settings that are handled by cmdr, i.e. layouts, favourites etc are not processed yet._

## Export from Traktor Settings using cmdr
Cmdr automatically extracts all needed information from a TSI file when you cut or copy single mappings or devices of it. 

# Make sure that Traktor is not running and backup your Traktor Settings TSI. 
# Open your Traktor Settings TSI. 
# Create a new TSI file.
# Copy&paste or drag&drop single mappings or whole devices to your newly created TSI. 
# Save the new TSI file.

## Import into Traktor Settings using cmdr
Cmdr automatically merges the existing settings with those of the file(s) to import. 

# Make sure that Traktor is not running and backup your Traktor Settings TSI. 
# Open the TSI file(s) you want to import and also your Traktor Settings TSI. 
# Copy&paste or drag&drop single mappings or whole devices to your Traktor Settings TSI. 
# Save your Traktor Settings TSI.
