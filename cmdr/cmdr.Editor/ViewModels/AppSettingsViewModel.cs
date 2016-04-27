using ChangeTracking;
using cmdr.Editor.AppSettings;
using cmdr.Editor.Utils;
using cmdr.TsiLib;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace cmdr.Editor.ViewModels
{
    public class AppSettingsViewModel : AReversible
    {
        private static readonly string WINDOW_TITLE = "cmdr Settings";

        private string _title = WINDOW_TITLE;
        public string Title
        {
            get { return _title; }
            set { _title = value; raisePropertyChanged("Title"); }
        }

        public Action CloseAction { get; set; }
        public Window Window { get; set; }

        private string _defaultWorkspace;
        public string DefaultWorkspace
        {
            get { return _defaultWorkspace; }
            set { SetProperty("DefaultWorkspace", ref _defaultWorkspace, ref value); }
        }

        private string _pathToControllerDefaultMappings;
        public string PathToControllerDefaultMappings
        {
            get { return _pathToControllerDefaultMappings; }
            set { SetProperty("PathToControllerDefaultMappings", ref _pathToControllerDefaultMappings, ref value); }
        }

        private string _pathToTraktorSettings;
        public string PathToTraktorSettings
        {
            get { return _pathToTraktorSettings; }
            set
            {
                if (SetProperty("PathToTraktorSettings", ref _pathToTraktorSettings, ref value)) 
                    updateTraktorVersion();
            }
        }

        private string _traktorVersion;
        public string TraktorVersion
        {
            get { return _traktorVersion; }
            set { SetProperty("TraktorVersion", ref _traktorVersion, ref value); }
        }

        private bool _mustOverrideTraktorVersion;
        public bool MustOverrideTraktorVersion
        {
            get { return _mustOverrideTraktorVersion; }
            set { _mustOverrideTraktorVersion = value; raisePropertyChanged("MustOverrideTraktorVersion"); }
        }

        private bool _overrideTraktorVersion;
        public bool OverrideTraktorVersion
        {
            get { return _overrideTraktorVersion; }
            set
            {
                _overrideTraktorVersion = value; 
                raisePropertyChanged("OverrideTraktorVersion"); 
                if (!value) 
                    restoreVersion();
            }
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

        private ICommand _browseFileCommand;
        public ICommand BrowseFileCommand
        {
            get { return _browseFileCommand ?? (_browseFileCommand = new CommandHandler<TextBlock>((tb) => browseFile(tb))); }
        }

        #endregion


        public AppSettingsViewModel()
        {
            DirtyStateChanged += onDirtyStateChanged;

            _defaultWorkspace = CmdrSettings.Instance.DefaultWorkspace ?? String.Empty;
            _pathToControllerDefaultMappings = CmdrSettings.Instance.PathToControllerDefaultMappings ?? String.Empty;
            _pathToTraktorSettings = CmdrSettings.Instance.PathToTraktorSettings ?? String.Empty;
            _traktorVersion = CmdrSettings.Instance.TraktorVersion ?? String.Empty;

            if (TraktorSettings.Initialized)
                _overrideTraktorVersion = !_traktorVersion.Equals(TraktorSettings.Instance.TraktorVersion);

            if (String.IsNullOrEmpty(CmdrSettings.Instance.PathToTraktorSettings))
            {
                _mustOverrideTraktorVersion = true;
                _overrideTraktorVersion = true;

                if (String.IsNullOrEmpty(CmdrSettings.Instance.TraktorVersion))
                    TraktorVersion = TraktorSettings.TRAKTOR_FALLBACK_VERSION;
            }
        }


        private void updateTraktorVersion()
        {
            MustOverrideTraktorVersion = String.IsNullOrEmpty(PathToTraktorSettings);

            if (!MustOverrideTraktorVersion)
            {
                var success = TraktorSettings.Initialize(PathToTraktorSettings, true);
                if (success)
                {
                    if (!OverrideTraktorVersion)
                        TraktorVersion = TraktorSettings.Instance.TraktorVersion;
                    else if (TraktorVersion.Equals(TraktorSettings.TRAKTOR_FALLBACK_VERSION))
                        OverrideTraktorVersion = false;
                }
            }
        }

        private void restoreVersion()
        {
            if (TraktorSettings.Initialized)
                TraktorVersion = TraktorSettings.Instance.TraktorVersion;
            else if (!String.IsNullOrEmpty(PathToTraktorSettings))
                updateTraktorVersion();
        }

        private void browseFolder(TextBlock textBlock)
        {
            string folder = BrowseDialogHelper.BrowseFolder(Window);
            if (folder != null)
                textBlock.Text = folder;
        }

        private void browseFile(TextBlock textBlock)
        {
            string folder = BrowseDialogHelper.BrowseTsiFile(Window, false);
            if (folder != null)
                textBlock.Text = folder;
        }

        private void save()
        {
            AcceptChanges();

            CmdrSettings.Instance.DefaultWorkspace = DefaultWorkspace;
            CmdrSettings.Instance.PathToControllerDefaultMappings = PathToControllerDefaultMappings;
            CmdrSettings.Instance.PathToTraktorSettings = PathToTraktorSettings;
            CmdrSettings.Instance.TraktorVersion = TraktorVersion;

            CmdrSettings.Instance.Save();

            if (CloseAction != null)
                CloseAction();
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
