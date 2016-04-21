using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdr.Editor.Utils
{
    static class BrowseDialogHelper
    {
        public static string BrowseFolder()
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            if (dlg.ShowDialog().GetValueOrDefault())
                return dlg.SelectedPath;
            return null;
        }
    }
}
