using System;
using System.Globalization;
using System.Reflection;
using cmdr.TsiLib.Commands.Interpretation;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;
using cmdr.TsiLib.Utils;

namespace cmdr.TsiLib.Commands
{
    public class CommandProxy : IMenuProxy
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


        internal ACommand Create(MappingSettings rawSettings)
        {
            var settings = rawSettings;

            Type makeType = null;
            if (MappingType == MappingType.In && _description.InCommandType != null)
                makeType = _description.InCommandType;
            else if (MappingType == MappingType.Out && _description.OutCommandType != null)
                makeType = _description.OutCommandType;
            else
                throw new Exception(String.Format("Command not supported:{0}-{1}" + MappingType, _description.Id));
           
            return (ACommand)Activator.CreateInstance(makeType, _flags, null, new object[] { _description.Id, _description.Name, _description.TargetType, settings }, _culture);
        }
    }
}
