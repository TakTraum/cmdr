using System;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.Controls.Button
{
    public abstract class AButtonControl : AControl
    {
        internal AButtonControl(ACommand command)
            : base(MappingControlType.Button, command)
        {

        }


        public override MappingInteractionMode[] AllowedInteractionModes
        {
            get
            {
                return new[]{
                    MappingInteractionMode.Trigger,
                    MappingInteractionMode.Direct, 
                    MappingInteractionMode.Hold, 
                    MappingInteractionMode.Increment,
                    MappingInteractionMode.Decrement,
                    MappingInteractionMode.Toggle,
                    MappingInteractionMode.Reset
                };
            }
        }

        public static AButtonControl FromCommand(ACommand command)
        {
            switch (command.InteractionMode)
            {
                case MappingInteractionMode.Trigger:
                    return new TriggerButtonControl(command);
                case MappingInteractionMode.Toggle:
                    return new ToggleButtonControl(command);
                case MappingInteractionMode.Hold:
                    return new HoldButtonControl(command);
                case MappingInteractionMode.Direct:
                case MappingInteractionMode.Reset:
                    return new DirectButtonControl(command);
                case MappingInteractionMode.Increment:
                case MappingInteractionMode.Decrement:
                    if (command.GetType().InheritsOrImplements(typeof(FloatInCommand<>)))
                        return new FloatDecIncButtonControl(command);
                    else
                        return new IntDecIncButtonControl(command);
                default:
                    return null;
            }
        }
    }
}
