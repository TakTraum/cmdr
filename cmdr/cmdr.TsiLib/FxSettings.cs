using cmdr.TsiLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Xml.Linq;
using cmdr.TsiLib.Utils;
using cmdr.TsiLib.FormatXml;
using cmdr.TsiLib.FormatXml.Interpretation;

namespace cmdr.TsiLib
{
    public class FxSettings
    {
        private List<Effect> _effects = new List<Effect>();
        public IReadOnlyList<Effect> Effects { get { return _effects; } }

        private Dictionary<Effect, FxSnapshot> _snapshots = new Dictionary<Effect, FxSnapshot>();
        public IReadOnlyDictionary<Effect, FxSnapshot> Snapshots { get { return _snapshots; } }


        internal FxSettings(List<Effect> effects, Dictionary<Effect, FxSnapshot> defaults)
        {
            _effects = effects;
            _snapshots = defaults;
        }


        public void SetSnapshot(FxSnapshot snapshot)
        {
            _snapshots[snapshot.Effect] = snapshot;
        }


        internal static FxSettings Load(TsiXmlDocument xml)
        {
            var fxSelection = xml.GetEntry<AudioFxSelection>();
            List<Effect> effects = (fxSelection != null) ? fxSelection.Value : new List<Effect>();

            Dictionary<Effect, FxSnapshot> defaults = new Dictionary<Effect, FxSnapshot>();
            var allEffects = Enum.GetValues(typeof(Effect)).Cast<Effect>().Except(new[] { Effect.NoEffect }).ToList();
            foreach (var effect in allEffects)
            {
                var effDef = FxSnapshot.Load(effect, xml);
                if (effDef != null)
                    defaults.Add(effect, effDef);
            }

            return new FxSettings(effects, defaults);
        }

        internal void Save(TsiXmlDocument xml)
        {
            var fxSelection = new AudioFxSelection { Value = _effects };
            xml.SaveEntry(fxSelection);

            foreach (var effDef in _snapshots.Values)
                effDef.Save(xml);
        }
    }
}
