using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdr.Editor.Utils
{
    static class BrowseDialogHelper
    {
        public static string BrowseFolder(System.Windows.Window owner)
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            if (dlg.ShowDialog(owner).GetValueOrDefault())
                return dlg.SelectedPath;
            return null;
        }

        public static string BrowseTsiFile(System.Windows.Window owner, bool isSaveDialog, string initialDirectory = null, string fileName = null, string type = "tsi")
        {
            VistaFileDialog dlg;

            if (isSaveDialog) {
                dlg = new VistaSaveFileDialog
                {
                    DefaultExt = type,
                    AddExtension = true,
                    ValidateNames = true
                };
            } else {
                dlg = new VistaOpenFileDialog
                {
                    Multiselect = false
                };
            }

            // fixme: make a class
            switch (type) {
                case "tsi":
                    dlg.Filter = "TSI | *.tsi";
                    break;
                case "csv":
                    dlg.Filter = "CSV | *.csv";
                    break;
            }
            dlg.CheckPathExists = true;

            if (initialDirectory != null) {
                dlg.InitialDirectory = initialDirectory;
                // workaround to set InitialDirectory
                dlg.FileName = Path.Combine(dlg.InitialDirectory, " ");
            }

            if (fileName != null) {
                fileName = System.IO.Path.ChangeExtension(fileName, "." + type);
                dlg.FileName = fileName;
            }

            if (dlg.ShowDialog(owner).GetValueOrDefault())
                return dlg.FileName;
            return null;
        }

        public static string MakeUnique(string path)
        {
            string dir = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string fileExt = Path.GetExtension(path);

            for (int i = 1; ; ++i) {
                if (!File.Exists(path))
                    return path;

                path = Path.Combine(dir, fileName + " (Backup " + i + ")" + fileExt);
            }
        }
    }
}
