using System;
using System.Globalization;
using System.Reflection;
using cmdr.TsiLib.Commands.Interpretation;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;

namespace cmdr.TsiLib.Commands
{
    public class CommandProxy
    {
        private static BindingFlags _flags = BindingFlags.NonPublic | BindingFlags.Instance;
        private static CultureInfo _culture = null; // use InvariantCulture or other if you prefer

        private CommandDescription _description;
        internal CommandDescription Description
        {
            get { return _description; }
        }

        public MappingType MappingType { get; private set; }
        public Categories Category { get { return _description.Category; } }
        public string Name { get { return _description.Name; } }


        internal CommandProxy(CommandDescription description, MappingType mappingType)
        {
            _description = description;
            MappingType = mappingType;
        }

        static Type prev_maketype = null;

        internal ACommand Create(MappingSettings rawSettings)
        {
            var settings = rawSettings;

            Type makeType = null;
            if (MappingType == MappingType.In && _description.InCommandType != null)
                makeType = _description.InCommandType;
            else if (MappingType == MappingType.Out && _description.OutCommandType != null)
                makeType = _description.OutCommandType;
            else
                throw new Exception(String.Format("Command not supported:{0}-{1}", MappingType, _description.Id));


            string n1 = makeType.Name;
            string n2 = makeType.FullName;
            string actual = "";

            try {
                actual = makeType.FullName.Split('[')[2].Split(' ')[0];
            }catch(Exception e) {

            }

            Console.WriteLine(n1);
            Console.WriteLine(n2);

            if (_description.Id == 3456) {
                var i = 9;

            } else {
                var i = 0;

            }

            // DDJ-T1: ArgumentException: An item with the same key has already been added.
            var new_obj = new object[] { _description.Id, _description.Name, _description.TargetType, settings };

            ACommand ret;
            //try {
                ret = (ACommand)Activator.CreateInstance(makeType, _flags, null, new_obj, _culture);
                prev_maketype = makeType;  //this is to delete after debug
                return ret;

            /*} 
            catch(Exception e) {

                return null;

            }*/
        }
    }
}
