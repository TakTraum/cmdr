using cmdr.TsiLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdr.TsiLib.FormatXml.Interpretation
{
    public class DefaultButtonFx : ListOfIntegerXmlEntry
    {
        public DefaultButtonFx(Effect effect)
            : base("DEFAULT_BUTTON_FX" + (int)effect)
        {

        }
    }
}
