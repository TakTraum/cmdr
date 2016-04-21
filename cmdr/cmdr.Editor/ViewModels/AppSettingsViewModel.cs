using ChangeTracking;
using cmdr.Editor.AppSettings;
using cmdr.Editor.Utils;
using cmdr.TsiLib;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace cmdr.Editor.ViewModels
{
    public class AppSettingsViewModel : AReversible
    {
        public struct InstalledTraktorVersion
        {
            public string Path { get; set; }
            public Version Version { get; set; }
        }

        private static readonly string WINDOW_TITLE = "cmdr Settings";
        private static readonly Regex REGEX_TRAKTOR_FOLDER = new Regex(@"^Traktor ([0-9\.]+)$");
        private static readonly string TRAKTOR_FALLBACK_VERSION = "2.0.1 (R10169)";

        private string _title = WINDOW_TITLE;
        public string Title
        {
            get { return _title; }
            set { _title = value; raisePropertyChanged("Title"); }
        }

        public Action CloseAction { get; set; }

        public string DefaultWorkspace
        {
            get { return CmdrSettings.Instance.DefaultWorkspace ?? String.Empty; }
            set { CmdrSettings.Instance.DefaultWorkspace = value; IsChanged = true; }
        }

        public string NativeInstrumentsFolder
        {
            get { return CmdrSettings.Instance.NativeInstrumentsFolder ?? String.Empty; }
            set { CmdrSettings.Instance.NativeInstrumentsFolder = value; updateTraktorVersions(); IsChanged = true; }
        }

        private ObservableCollection<string> _traktorVersions = new ObservableCollection<string>();
        public ObservableCollection<string> TraktorVersions
        {
            get { return _traktorVersions; }
            set { _traktorVersions = value; raisePropertyChanged("TraktorVersions"); }
        }

        private string _selectedTraktorVersion;
        public string SelectedTraktorVersion
        {
            get { return _selectedTraktorVersion; }
            set { _selectedTraktorVersion = value; raisePropertyChanged("SelectedTraktorVersion"); updateTraktorVersion(); }
        }

        private bool _overrideTraktorVersion;
        public bool OverrideTraktorVersion
        {
            get { return _overrideTraktorVersion; }
            set { _overrideTraktorVersion = value; raisePropertyChanged("OverrideTraktorVersion"); }
        }


        #region Commands

        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new CommandHandler(CloseAction)); }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new CommandHandler(save, () => IsChanged)); }
        }

        private ICommand _browseFolderCommand;
        public ICommand BrowseFolderCommand
        {
            get { return _browseFolderCommand ?? (_browseFolderCommand = new CommandHandler<TextBlock>((tb) => browseFolder(tb))); }
        }

        #endregion


        public AppSettingsViewModel()
        {
            DirtyStateChanged += onDirtyStateChanged;

            if (!CmdrSettings.Instance.TraktorSection.InstalledVersions.Any())
                updateTraktorVersions();
            else
                _traktorVersions = new ObservableCollection<string>(CmdrSettings.Instance.TraktorSection.InstalledVersions.Select(v => v.Version));

            if (!TraktorVersions.Any(v => v.Equals(CmdrSettings.Instance.TraktorSection.SelectedVersion)))
                _overrideTraktorVersion = true;
            else if (!String.IsNullOrWhiteSpace(CmdrSettings.Instance.TraktorSection.SelectedVersion))
                _selectedTraktorVersion = CmdrSettings.Instance.TraktorSection.SelectedVersion;

            if (!CmdrSettings.Instance.Initialized)
            {
                _selectedTraktorVersion = TRAKTOR_FALLBACK_VERSION;
                IsChanged = true;
            }
            else
                _selectedTraktorVersion = CmdrSettings.Instance.TraktorSection.SelectedVersion;
        }


        private void updateTraktorVersions()
        {
            if (Directory.Exists(NativeInstrumentsFolder))
            {
                var traktorFolders = Directory.EnumerateDirectories(NativeInstrumentsFolder, "Traktor *")
                    .Select(f => new DirectoryInfo(f));

                var installedVersions = traktorFolders
                    .Where(d => REGEX_TRAKTOR_FOLDER.IsMatch(d.Name))
                    .Select(d => new InstalledTraktorVersion{ Path = d.FullName, Version = new Version(REGEX_TRAKTOR_FOLDER.Match(d.Name).Groups[1].Value) })
                    .OrderByDescending(itv => itv.Version);

                foreach (var tv in installedVersions)
                {
                    CmdrSettings.Instance.TraktorSection.InstalledVersions.Add(
                        new InstalledTraktorVersionConfigElement
                        {
                            Version = tv.Version.ToString(),
                            Path = tv.Path
                        });
                }

                TraktorVersions = new ObservableCollection<string>(installedVersions.Select(v => v.Version.ToString()));
            }
        }

        private void updateTraktorVersion()
        {
            CmdrSettings.Instance.TraktorSection.SelectedVersion = SelectedTraktorVersion;
            initializeTraktorSettings();
        }

        private void browseFolder(TextBlock textBlock)
        {
            string folder = BrowseDialogHelper.BrowseFolder();
            if (folder != null)
                textBlock.Text = folder;
        }

        private void save()
        {
            AcceptChanges();

            if (!CmdrSettings.Instance.Initialized)
                CmdrSettings.Instance.TraktorSection.SelectedVersion = _selectedTraktorVersion;

            CmdrSettings.Instance.Save();

            if (CloseAction != null)
                CloseAction();
        }

        private void initializeTraktorSettings()
        {
            if (CmdrSettings.Instance.TraktorSection.TraktorFolder != null)
            {
                string pathToTraktorSettings = Path.Combine(CmdrSettings.Instance.TraktorSection.TraktorFolder, "Traktor Settings.tsi");
                var success = TraktorSettings.Initialize(pathToTraktorSettings);
            }
        }

        private void refreshAppTitle()
        {
            Title = IsChanged ? WINDOW_TITLE + "*" : WINDOW_TITLE;
        }


        void onDirtyStateChanged(object sender, bool e)
        {
            refreshAppTitle();
        }

        protected override void Accept()
        {

        }

        protected override void Revert()
        {

        }
    }
}
