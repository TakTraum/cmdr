using cmdr.TsiLib.Utils;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace cmdr.TsiLib.Format
{
    internal class DVST : Frame
    {
        /* Normally seems to be:
        F890h: 44 56 53 54 00 00 00 14 00 00 00 01 00 00 00 00  DVST............ 
        F8A0h: 00 00 00 00 00 00 00 00 00 00 00 00              ............
        
        but for F1 it's:    
        0AE0h: 44 56 53 54 00 00 00 62 00 00 00 01 00 00 00 00  DVST...b........ 
        0AF0h: 00 00 00 00 00 00 00 4E 00 00 00 4E 3C 3F 78 6D  .......N...N<?xm 
        0B00h: 6C 20 76 65 72 73 69 6F 6E 3D 22 31 2E 30 22 20  l version="1.0"  
        0B10h: 65 6E 63 6F 64 69 6E 67 3D 22 75 74 66 2D 38 22  encoding="utf-8" 
        0B20h: 3F 3E 0A 3C 46 31 3E 3C 41 73 73 69 67 6E 65 64  ?>.<F1><Assigned 
        0B30h: 44 65 63 6B 3E 34 3C 2F 41 73 73 69 67 6E 65 64  Deck>4</Assigned 
        0B40h: 44 65 63 6B 3E 3C 2F 46 31 3E                    Deck></F1>
        */

        public bool Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public int Unknown4 { get; set; }
        public string Xml { get; set; }


        public DVST()
            : base("DVST")
        {
            Unknown1 = true;
            Xml = String.Empty;
        }

        public DVST (Stream stream)
            : base(stream)
        {
            Unknown1 = stream.ReadBoolBigE();
            Unknown2 = stream.ReadInt32BigE();
            Unknown3 = stream.ReadInt32BigE();
            Unknown4 = stream.ReadInt32BigE();

            var len = stream.ReadInt32BigE();
            Xml = stream.ReadASCIIString(len);
        }


        public override void Write(Writer writer)
        {
            writer.BeginFrame(FrameId);

            writer.WriteBigE(Unknown1);
            writer.WriteBigE(Unknown2);
            writer.WriteBigE(Unknown3);
            writer.WriteBigE(Unknown4);

            writer.WriteBigE(Xml.Length);
            writer.WriteAsciiBigE(Xml);

            writer.EndFrame();
        }


        private void setContent()
        {
            var template = getTemplate("Traktor Kontrol F1");
            if (template != null)
            {
                // set value
                string XPATH_TO_DATA = "/F1";
                var element = template.XPathSelectElement(XPATH_TO_DATA);
                element.SetElementValue("AssignedDeck", Enums.DeviceTarget.Focus);
                var clean = template.ToString();
                Xml = Regex.Replace(clean, @"\t|\n|\r", "");
            }
        }

        private XDocument getTemplate(string deviceName)
        {
            var resourceName = "DVST_" + deviceName;
            if (EmbeddedResource.Contains(resourceName))
                using (Stream source = EmbeddedResource.Get(resourceName))
                    return XDocument.Load(source);
            return null;
        }
    }
}
