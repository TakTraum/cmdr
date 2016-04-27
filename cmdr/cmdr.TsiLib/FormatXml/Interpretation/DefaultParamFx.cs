using cmdr.TsiLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdr.TsiLib.FormatXml.Interpretation
{
    public class DefaultParamFx : ListOfFloatXmlEntry
    {
        public DefaultParamFx(Effect effect)
            : base("DEFAULT_PARAM_FX" + (int)effect)
        {

        }
    }
}
