using cmdr.Editor.AppSettings;
using cmdr.TsiLib.MidiDefinitions.Base;
using cmdr.WpfControls.DropDownButton;
using cmdr.WpfControls.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace cmdr.Editor.ViewModels.MidiBinding
{
    class NotesMenuBuilder
    {
        private const int CC_MAX = 128;
        private const int CC_PARTS = 4;
        private const int CC_PART = CC_MAX / CC_PARTS;

        private readonly List<string> NOTENAMES = new List<string> { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        private readonly MenuBuilder<string> _genericMenuBuilder = new MenuBuilder<string>();
        private readonly MenuBuilder<AMidiDefinition> _proprietaryMenuBuilder = new MenuBuilder<AMidiDefinition>();


        private NotesMenuBuilder() { }

        private static NotesMenuBuilder _instance;
        public static NotesMenuBuilder Instance
        {
            get { return _instance ?? (_instance = new NotesMenuBuilder()); }
        }


        public List<MenuItemViewModel> BuildGenericMidiMenu()
        {
            MenuItemViewModel root = new MenuItemViewModel();

            // None
            root.Children.Add(new MenuItemViewModel { Text = "None", Tag = "None" });

            #region CC

            MenuItemViewModel rangeMenu = null;
            int part = 0;
            int limit = 0;
            string ccNumString;
            string ccString;
            for (int i = 0; i < CC_MAX; i++)
            {
                if (i == limit)
                {
                    part++;
                    limit = CC_PART * part;
                    rangeMenu = new MenuItemViewModel { Text = String.Format("CC {0} - {1}", i, limit - 1) };
                    root.Children.Add(rangeMenu);
                }

                ccNumString = String.Format("{0:000}", i);
                ccString = "CC." + ccNumString;
                rangeMenu.Children.Add(new MenuItemViewModel { Text = ccNumString, Tag = ccString });
            }

            #endregion

            #region Notes
            
            bool add_count = CmdrSettings.Instance.ShowDecimalNotes;
            int count = 0;
            int maxOctave;
            var specialNotes = new[] { "G#", "A", "A#", "B" };
            MenuItemViewModel noteMenu = null;
            foreach (var note in NOTENAMES)
            {
                noteMenu = new MenuItemViewModel { Text = note };
                root.Children.Add(noteMenu);

                maxOctave = specialNotes.Contains(note) ? 8 : 9;

                for (int i = -1; i <= maxOctave; i++)
                {
                    String text;
                    String tag;

                    if (add_count)
                    {
                        text = String.Format("{0} ({1})", i.ToString(), count);
                        //tag = String.Format("Note.{0} ({1})", note + i, count);
                        tag = String.Format("Note.{0}", note + i);
                    }
                    else {
                        text = String.Format("{0}", i.ToString());
                        tag = String.Format("Note.{0}", note + i);

                    }

                    noteMenu.Children.Add(new MenuItemViewModel { Text = text, Tag = tag });
                    count++;
                }
            }

            #endregion

            // PitchBend
            root.Children.Add(new MenuItemViewModel { Text = "PitchBend", Tag = "PitchBend" });

            return root.Children;
        }

        public List<MenuItemViewModel> BuildProprietaryMenu(IEnumerable<AMidiDefinition> proprietaryDefinitions)
        {
            var itemBuilder = new Func<AMidiDefinition, MenuItemViewModel>(d => new MenuItemViewModel
            {
                Text = d.Note.Split('.').Last().Trim(),
                Tag = d
            });

            return _proprietaryMenuBuilder.BuildTree(proprietaryDefinitions, itemBuilder, d => d.Note, ".", true).ToList();
        }

        public List<MenuItemViewModel> BuildSelectedNotesMenu(IEnumerable<object> selectedNotes)
        {
            if (selectedNotes.First() is AMidiDefinition)
                return BuildProprietaryMenu(selectedNotes.Cast<AMidiDefinition>());
            return _genericMenuBuilder.BuildList(selectedNotes.Cast<string>(), buildMenuItemSelected);  // , buildPath, ".", false);
        }


        private string buildPath(string note)
        {
            string category = note;

            if (note.Contains("+"))
                category = "Combo";
            else if (note.StartsWith("CC."))
            {
                int ccNum = Int32.Parse(note.Split('.').Last());
                int lower = 0;
                while (ccNum > lower + CC_PART)
                    lower += CC_PART;
                int upper = lower + CC_PART - 1;

                category = "CC." + String.Format("{0} - {1}", lower, upper);
            }
            else if (note.StartsWith("Note."))
                category = "Note." + Regex.Match(note, @"([A-G#]+)").Groups[1].Value;

            return category;
        }

        private MenuItemViewModel buildMenuItem(string note)
        {
            string text = note.Split('.').Last();
            if (note.Contains("+"))
                text = note;
            else if (note.StartsWith("Note"))
                text = Regex.Match(note, @"([-\d]+)").Groups[1].Value;

            return new MenuItemViewModel
            {
                Text = text,
                Tag = note
            };
        }

        private MenuItemViewModel buildMenuItemSelected(string note)
        {
            string text = note;
            return new MenuItemViewModel
            {
                Text = text,
                Tag = note
            };
        }
    }
}
