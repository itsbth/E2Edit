using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
        private static string _e2Path;
        private string _currentFile;

        public MainWindow()
        {
            IHighlightingDefinition definition = HighlightingLoader.Load(XmlReader.Create("Expression2.xshd"),
                                                 HighlightingManager.Instance);
            HighlightingManager.Instance.RegisterHighlighting("Expression2", new[] { ".txt" }, definition);

            InitializeComponent();

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (s, e) => Close()));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Save, (s, e) => e.CanExecute = _currentFile != null));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, SaveAs));

            if (File.Exists("SteamPath.txt"))
            {
                _e2Path = File.ReadAllText("SteamPath.txt").Trim();
            }
            else
            {
                var dlg = new VistaFolderBrowserDialog {Description = Properties.Resources.MainWindow_MainWindow_Select_the_E2_Data_folder};
                if (dlg.ShowDialog() == true)
                {
                    _e2Path = dlg.SelectedPath;
                    File.WriteAllText("SteamPath.txt", _e2Path);
                }
                else
                {
                    Close();
                }
            }
            if (!_e2Path.EndsWith(@"\")) _e2Path += @"\";

            foreach (string file in Directory.GetFiles(_e2Path))
            {
                _fileList.Items.Add(Path.GetFileName(file));
            }
        }

        private void Save(object sender, ExecutedRoutedEventArgs e)
        {
            if (_currentFile != null)
            {
                _editor.Save(_currentFile);
            }
            else
            {
                SaveAs(sender, e);
            }
        }

        private void FileListMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _editor.Open(_e2Path + _fileList.SelectedItem);
            _currentFile = _e2Path + _fileList.SelectedItem;
            Title = _fileList.SelectedItem + " - E2Edit";
        }

        private void DoDragMove(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void SaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            // This really needs some refactoring...
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
            if (!((bool)diag.ShowDialog())) return;
// ReSharper restore PossibleInvalidOperationException
            string fname = diag.FileName;
            if (!fname.EndsWith(".txt", StringComparison.CurrentCultureIgnoreCase)) fname += ".txt";
            if (File.Exists(_e2Path + fname))
            {
                if (MessageBox.Show(String.Format(Properties.Resources.MainWindow_SaveAs_File_already_exists, fname), "Save As", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            _currentFile = _e2Path + fname;
            Save(sender, e);
        }
    }
}