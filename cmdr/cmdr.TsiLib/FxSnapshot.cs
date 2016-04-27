using cmdr.TsiLib.Enums;
using cmdr.TsiLib.FormatXml;
using cmdr.TsiLib.FormatXml.Interpretation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdr.TsiLib
{
    public class FxSnapshot
    {
        public class FxButtonsSnapshot
        {
            public int ButtonGroupMode { get; set; }
            public int Button3 { get; set; }
            public int Button2 { get; set; }
            public int Button1 { get; set; }
            public int OnOff { get; set; }
        }

        public class FxKnobsSnapshot
        {
            public float KnobGroupMode { get; set; }
            public float Knob3 { get; set; }
            public float Knob2 { get; set; }
            public float Knob1 { get; set; }
            public float DryWet { get; set; }
        }


        private readonly Effect _effect;
        public Effect Effect
        {
            get { return _effect; }
        } 

        private FxButtonsSnapshot _buttons;
        public FxButtonsSnapshot Buttons
        {
            get { return _buttons ?? (_buttons = new FxButtonsSnapshot());}
        }

        private FxKnobsSnapshot _knobs;
        public FxKnobsSnapshot Knobs
        {
            get { return _knobs ?? (_knobs = new FxKnobsSnapshot());}
        }


        public FxSnapshot(Effect effect)
        {
            _effect = effect;
        }


        internal static FxSnapshot Load(Effect effect, TsiXmlDocument xml)
        {
            var fxDefault = new FxSnapshot(effect);

            DefaultButtonFx defBtn = xml.GetEntry(new DefaultButtonFx(effect));
            if (defBtn != null)
            {
                fxDefault._buttons = new FxButtonsSnapshot
                {
                    ButtonGroupMode = defBtn.Value[0],
                    Button3 = defBtn.Value[1],
                    Button2 = defBtn.Value[2],
                    Button1 = defBtn.Value[3],
                    OnOff = defBtn.Value[4]
                };
            }

            DefaultParamFx defParam = xml.GetEntry(new DefaultParamFx(effect));
            if (defParam != null)
            {
                fxDefault._knobs = new FxKnobsSnapshot
                {
                    KnobGroupMode = defParam.Value[0],
                    Knob3 = defParam.Value[1],
                    Knob2 = defParam.Value[2],
                    Knob1 = defParam.Value[3],
                    DryWet = defParam.Value[4]
                };
            }

            // reason for logical OR: usually, there is both entries, but preservation goes over completeness
            // TODO: try if Traktor accepts "half" snapshots
            if (defBtn != null || defParam != null)
                return fxDefault;

            return null;
        }


        internal void Save(TsiXmlDocument xml)
        {
            var defBtn = new DefaultButtonFx(_effect)
            {
                Value = new List<int> 
                            { 
                                Buttons.ButtonGroupMode, 
                                Buttons.Button3, 
                                Buttons.Button2, 
                                Buttons.Button1, 
                                Buttons.OnOff, 
                            }
            };
            xml.SaveEntry(defBtn);

            var defParam = new DefaultParamFx(_effect)
            {
                Value = new List<float>
                            {
                                Knobs.KnobGroupMode,
                                Knobs.Knob3,
                                Knobs.Knob2,
                                Knobs.Knob1,
                                Knobs.DryWet,
                            }
            };
            xml.SaveEntry(defParam);
        }
    }
}
