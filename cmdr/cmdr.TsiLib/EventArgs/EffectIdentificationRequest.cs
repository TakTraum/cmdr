
namespace cmdr.TsiLib.EventArgs
{
    public class EffectIdentificationRequest : System.EventArgs
    {
        public string Id { get; private set; }
        public FxSettings FxSettings { get; set; }
        public bool Handled { get; set; }

        public EffectIdentificationRequest(string id)
        {
            Id = id;
        }
    }
}
