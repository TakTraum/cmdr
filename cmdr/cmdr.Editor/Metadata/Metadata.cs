using System.Reflection;

namespace cmdr.Editor.Metadata
{
    public class Metadata
    {
        public string Version { get; set; }
        public DeviceMetadata DeviceMetadata { get; set; }


        public Metadata()
        {
            Version = Assembly.GetAssembly(this.GetType()).GetName().Version.ToString();
            DeviceMetadata = new DeviceMetadata();
        }
    }
}
