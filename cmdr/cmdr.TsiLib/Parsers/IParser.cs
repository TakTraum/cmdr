
namespace cmdr.TsiLib.Parsers
{
    public interface IParser<T>
    {
        T DecodeValue(byte[] rawValue);
        byte[] EncodeValue(T value);
    }
}
