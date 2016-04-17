using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.Controls
{
    public class All
    {
        private static Type[] _root = { 
                                   typeof(Button.DirectButtonControl),
                                   typeof(Button.IntDecIncButtonControl),
                                   typeof(Button.FloatDecIncButtonControl),
                                   typeof(Button.HoldButtonControl),
                                   typeof(Button.ToggleButtonControl),
                                   typeof(Button.TriggerButtonControl),
                                   typeof(FaderOrKnob.DirectFaderOrKnobControl),
                                   typeof(FaderOrKnob.RelativeFaderOrKnobControl),
                                   typeof(Encoder.EncoderControl),
                                   typeof(LED.LedControl<Int32>)
                                   };

        private static BindingFlags _flags = BindingFlags.NonPublic | BindingFlags.Instance;
        private static CultureInfo _culture = CultureInfo.CurrentCulture;


        internal static AControl GetControl(ACommand command)
        {
            switch (command.ControlType)
            {
                case MappingControlType.Button:
                    return Button.AButtonControl.FromCommand(command);
                case MappingControlType.FaderOrKnob:
                    return FaderOrKnob.FaderOrKnobControl.FromCommand(command);
                case MappingControlType.Encoder:
                    return new Encoder.EncoderControl(command);
                case MappingControlType.LED:
                    var d1 = typeof(LED.LedControl<>);
                    var t = command.GetType();
                    while (!t.IsGenericType && t.BaseType != typeof(object))
                        t = t.BaseType;
                    if (t.IsGenericType)
                    {
                        var makeme = d1.MakeGenericType(t.GenericTypeArguments);
                        return Activator.CreateInstance(makeme, _flags, null, new object[] { command }, _culture) as AControl;
                    }
                    else
                    {
                        return new LED.LedControl<int>(command);
                    }
                default:
                    return null;
            }
        }


        public static AControl[] GetControls(ACommand command, params MappingInteractionMode[] interactionModes)
        {
            var controls = _root.Select(c => Activator.CreateInstance(c, _flags, null, new object[] { command }, _culture) as AControl).Where(c => !c.AllowedInteractionModes.Except(interactionModes).Any());

            if (controls.Any( c => c.Type == MappingControlType.FaderOrKnob) && !interactionModes.Contains(MappingInteractionMode.Relative))
                controls = controls.Except(controls.Where(c => c.Type == MappingControlType.FaderOrKnob));
         
            return controls.ToArray();
        }
    }
}
