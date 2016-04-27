using cmdr.TsiLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace cmdr.TsiLib.FormatXml.Interpretation
{
    public class AudioFxSelection : ListOfEnumXmlEntry<Effect>
    {
        public AudioFxSelection()
            : base("Audio.FX.Selection")
        {

        }
    }
}
