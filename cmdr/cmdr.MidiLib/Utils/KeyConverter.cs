using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdr.MidiLib.Utils
{
    public class KeyConverter
    {
        public readonly List<string> NOTES = new List<string> { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

        /// <summary>
        /// Gets the key text.
        /// </summary>
        /// <param name="key">Key 0 - 127</param>
        /// <returns>e.g. D#2</returns>
        public string GetKeyText(int key)
        {
            return GetNote(key) + GetOctave(key);
        }

        /// <summary>
        /// Gets the key text using International Pitch Notation.
        /// </summary>
        /// <param name="key">Key 0 - 127</param>
        /// <returns>e.g. D#-1</returns>
        public string GetKeyTextIPN(int key)
        {
            return GetNote(key) + GetOctaveIPN(key);
        }

        /// <summary>
        /// Gets note as string.
        /// </summary>
        /// <param name="key">Key 0 - 127</param>
        /// <returns>e.g. D#</returns>
        public string GetNote(int key)
        {
            return NOTES[GetNoteNumber(key)];
        }

        /// <summary>
        /// Gets note number.
        /// </summary>
        /// <param name="key">Key 0 - 127</param>
        /// <returns>e.g. 0, which corresponds to C.</returns>
        public int GetNoteNumber(int key)
        {
            return key % 12;
        }

        /// <summary>
        /// Gets the octave using International Pitch Notation.
        /// </summary>
        /// <param name="key">Key 0 - 127</param>
        /// <returns>Ocatve starting at -1.</returns>
        public int GetOctaveIPN(int key)
        {
            return GetOctave(key) - 1;
        }

        /// <summary>
        /// Gets the octave.
        /// </summary>
        /// <param name="key">Key 0 - 127</param>
        /// <returns>Octave starting at 0.</returns>
        public int GetOctave(int key)
        {
            return key / 12;
        }

        /// <summary>
        /// Converts note number and octave to key using International Pitch Notation.
        /// </summary>
        /// <param name="note">Note C - B</param>
        /// <param name="octave">Octave -1 - 9</param>
        /// <returns>Key 0 - 127</returns>
        public int ToKeyIPN(string note, int octave)
        {
            return ToKeyIPN(NOTES.IndexOf(note.ToUpper()), octave);
        }

        /// <summary>
        /// Converts note number and octave to key using International Pitch Notation.
        /// </summary>
        /// <param name="noteNumber">Note number 0 - 11</param>
        /// <param name="octave">Octave -1 - 9</param>
        /// <returns>Key 0 - 127</returns>
        public int ToKeyIPN(int noteNumber, int octave)
        {
            return ToKey(noteNumber, octave) + 12;
        }

        /// <summary>
        /// Converts note number and octave to key.
        /// </summary>
        /// <param name="note">Note C - B</param>
        /// <param name="octave">Octave 0 - 10</param>
        /// <returns>Key 0 - 127</returns>
        public int ToKey(string note, int octave)
        {
            return (octave * 12 + NOTES.IndexOf(note.ToUpper()));
        }

        /// <summary>
        /// Converts note number and octave to key.
        /// </summary>
        /// <param name="noteNumber">Note number 0 - 11</param>
        /// <param name="octave">Octave 0 - 10</param>
        /// <returns>Key 0 - 127</returns>
        public int ToKey(int noteNumber, int octave)
        {
            return (octave * 12 + noteNumber);
        }
    }
}
