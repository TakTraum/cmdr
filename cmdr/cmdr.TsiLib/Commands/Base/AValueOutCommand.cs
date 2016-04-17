using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;

namespace cmdr.TsiLib.Commands
{
    public abstract class AValueOutCommand<T> : AValueCommand<T>
    {        
        /// <summary>
        /// Controller Range Minimum depending on data type of command.
        /// </summary>
        public T ControllerRangeMin
        {
            get { return Parser.DecodeValue(RawSettings.LedMinControllerRange); }
            set { RawSettings.LedMinControllerRange = Parser.EncodeValue(value); }
        }

        /// <summary>
        /// Controller Range Maximum depending on data type of command.
        /// </summary>
        public T ControllerRangeMax
        {
            get { return Parser.DecodeValue(RawSettings.LedMaxControllerRange); }
            set { RawSettings.LedMaxControllerRange = Parser.EncodeValue(value); }
        }


        internal AValueOutCommand(int id, string name, TargetType target, MappingSettings rawSettings)
            : base(id, name, target, MappingType.Out, rawSettings)
        {
            RawSettings.ControlType = MappingControlType.LED;
            RawSettings.InteractionMode = MappingInteractionMode.Output;
            
            RawSettings.ValueUIType = ValueUIType.ComboBox;

            if (RawSettings.LedMinControllerRange == null)
                ControllerRangeMin = GetDefaultControllerRangeMin();

            if (RawSettings.LedMaxControllerRange == null)
                ControllerRangeMax = GetDefaultControllerRangeMax();
        }


        protected override MappingInteractionMode[] AllowedInteractionModes
        {
            get { return new MappingInteractionMode[] { MappingInteractionMode.Output }; }
        }

        protected abstract T GetDefaultControllerRangeMin();
        protected abstract T GetDefaultControllerRangeMax();
    }
}
