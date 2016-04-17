using System.IO;
using System.Linq;
using System.Reflection;

namespace cmdr.TsiLib
{
    internal static class EmbeddedResource
    {
        static Assembly ASSEMBLY = Assembly.GetExecutingAssembly();
        static string PREFIX = ASSEMBLY.GetName().Name + ".Resources.";
        static string[] EMBEDDED_RESOURCENAMES = null;


        internal static bool Contains(string resourceName)
        {
            if (EMBEDDED_RESOURCENAMES == null)
                EMBEDDED_RESOURCENAMES = ASSEMBLY.GetManifestResourceNames();
            return EMBEDDED_RESOURCENAMES.Contains(PREFIX + resourceName);
        }

        internal static Stream Get(string resourceName)
        {
            if (EMBEDDED_RESOURCENAMES == null)
                EMBEDDED_RESOURCENAMES = ASSEMBLY.GetManifestResourceNames();
            return ASSEMBLY.GetManifestResourceStream(PREFIX + resourceName);
        }
    }
}
