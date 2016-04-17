
namespace cmdr.TsiLib.Ranges
{
    public abstract class ARange<T>
    {
        public T MinValue { get; private set; }
        public T MaxValue { get; private set; }


        public ARange(T minValue, T maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }
}
