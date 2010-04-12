using System.IO;
using System.Windows;
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
        public static string E2Path;

        public MainWindow()
        {
            IHighlightingDefinition definition = HighlightingLoader.Load(XmlReader.Create("Expression2.xshd"),
                                                 HighlightingManager.Instance);
            HighlightingManager.Instance.RegisterHighlighting("Expression2", new[] { ".txt" }, definition);

            InitializeComponent();

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (s, e) => Close()));

            if (File.Exists("SteamPath.txt"))
            {
                E2Path = File.ReadAllText("SteamPath.txt").Trim();
            }
            else
            {
                var dlg = new VistaFolderBrowserDialog {Description = Properties.Resources.MainWindow_MainWindow_Select_the_E2_Data_folder};
                if (dlg.ShowDialog() == true)
                {
                    E2Path = dlg.SelectedPath;
                    File.WriteAllText("SteamPath.txt", E2Path);
                }
                else
                {
                    Close();
                }
            }
            if (!E2Path.EndsWith(@"\")) E2Path += @"\";

            foreach (string file in Directory.GetFiles(E2Path))
            {
                _fileList.Items.Add(Path.GetFileName(file));
            }
        }

        private void FileListMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _editor.Open(E2Path + _fileList.SelectedItem);
            Title = _fileList.SelectedItem + " - E2Edit";
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}