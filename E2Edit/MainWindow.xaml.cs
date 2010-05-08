using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Ookii.Dialogs.Wpf;

namespace E2Edit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _currentFile;
        internal static Settings Settings;

        public MainWindow()
        {
            IHighlightingDefinition definition = HighlightingLoader.Load(XmlReader.Create("Expression2.xshd"),
                                                                         HighlightingManager.Instance);
            HighlightingManager.Instance.RegisterHighlighting("Expression2", new[] {".txt"}, definition);

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (s, e) => Close()));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Save));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, SaveAs));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, New));
            //CommandBindings.Add(new CommandBinding(ApplicationCommands.Help, (s, e) => new SettingsDialog(Settings).ShowDialog()));

            Closing += CheckSaveState;

            if (File.Exists("Settings.xml"))
            {
                using (Stream fs = new FileStream("Settings.xml", FileMode.Open))
                {
                    Settings = Settings.Load(fs);
                }
            }
            else
            {
                Settings = new Settings();
                //new SettingsDialog(Settings).ShowDialog();
                using (Stream fs = new FileStream("Settings.xml", FileMode.OpenOrCreate))
                {
                    Settings.Save(fs);
                }
            }
            if (String.IsNullOrEmpty(Settings.SteamPath))
            {
                if(!FindSteamPath())
                {
                    Close();
                    return;
                }
                using (Stream fs = new FileStream("Settings.xml", FileMode.OpenOrCreate))
                {
                    Settings.Save(fs);
                }
            }
            if (!Settings.SteamPath.EndsWith(@"\")) Settings.SteamPath += @"\";
            InitializeComponent();
            UpdateFileList();
        }

        private static bool FindSteamPath()
        {
            if (File.Exists("SteamPath.txt"))
            {
                Settings.SteamPath = File.ReadAllText("SteamPath.txt").Trim();
            }
            else
            {
                var dlg = new VistaFolderBrowserDialog { Description = Properties.Resources.MainWindow_MainWindow_Select_the_E2_Data_folder };
                if (dlg.ShowDialog() == true)
                {
                    Settings.SteamPath = dlg.SelectedPath;
                }
                else
                {
                    MessageBox.Show(Properties.Resources.MainWindow_MainWindow_You_have_to_specify_a_folder);
                    return false;
                }
            }
            return true;
        }

        private void CheckSaveState(object sender, CancelEventArgs e)
        {
            if (ShouldSave()) return;
            e.Cancel = true;
        }

        private void UpdateFileList()
        {
            _fileList.Items.Clear();
            foreach (string file in Directory.GetFiles(Settings.SteamPath))
            {
                _fileList.Items.Add(Path.GetFileName(file));
            }
        }

        private void FileListMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!ShouldSave()) return;
            _editor.Open(Settings.SteamPath + _fileList.SelectedItem);
            _currentFile = Settings.SteamPath + _fileList.SelectedItem;
            Title = _fileList.SelectedItem + " - E2Edit";
        }

        private bool ShouldSave()
        {
            if (!_editor.IsModified) return true;
            MessageBoxResult result =
                MessageBox.Show(Properties.Resources.MainWindow_ShouldSave_Save_changes_to_the_current_file, "E2Edit",
                                MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    Save();
                    break;
                case MessageBoxResult.Cancel:
                    return false;
            }
            return true;
        }

        private void DoDragMove(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void New(object sender, ExecutedRoutedEventArgs e)
        {
            _currentFile = null;
            _editor.Text = String.Empty;
        }

        private void Save(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
        }

        private void Save()
        {
            if (_currentFile != null)
            {
                _editor.Save(_currentFile);
            }
            else
            {
                SaveAs();
            }
        }

        private void SaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            // This really needs some refactoring...
            SaveAs();
        }

        private void SaveAs()
        {
            if (_editor.Text.IndexOf("Led1 = 1,0,1,0,1,0,1,0,1") != -1)
            {
                var parent = (DockPanel) _editor.Parent;
                parent.Children.Remove(_editor);
                var pongGame = new PongGame();
                parent.Children.Add(pongGame);
                pongGame.SetupGame(1);
                return;
            }
            var diag = new SaveAsDialog();
// ReSharper disable PossibleInvalidOperationException
            if (!((bool) diag.ShowDialog())) return;
// ReSharper restore PossibleInvalidOperationException
            string fname = diag.FileName;
            if (!fname.EndsWith(".txt", StringComparison.CurrentCultureIgnoreCase)) fname += ".txt";
            if (File.Exists(Settings.SteamPath + fname))
            {
                if (
                    MessageBox.Show(String.Format(Properties.Resources.MainWindow_SaveAs_File_already_exists, fname),
                                    "Save As", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            _currentFile = Settings.SteamPath + fname;
            Save();
            UpdateFileList();
        }
    }
}