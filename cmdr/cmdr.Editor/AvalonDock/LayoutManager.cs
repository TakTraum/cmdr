using System.IO;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace cmdr.Editor.AvalonDock
{
    public class LayoutManager
    {
        private static readonly string LAYOUT_PATH = "Layout.xml";
        private readonly DockingManager _dockingManager;
        private readonly XmlLayoutSerializer _serializer;

        public bool DefaultLayoutAvailable { get { return File.Exists(LAYOUT_PATH); } }


        public LayoutManager(DockingManager dockingManager)
        {
            _dockingManager = dockingManager;
            _serializer = new XmlLayoutSerializer(dockingManager);
        }


        public void LoadLayout()
        {
            if (!DefaultLayoutAvailable)
                return;

            using (var stream = new StreamReader(LAYOUT_PATH))
                _serializer.Deserialize(stream);
        }


        public void SaveLayout()
        {
            using (var stream = new StreamWriter(LAYOUT_PATH))
                _serializer.Serialize(stream);
        }
    }
}
