using System.IO;
using System.Windows;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace E2Edit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string E2Path = @"C:\Games\Steam\steamapps\itsbth\garrysmod\garrysmod\data\Expression2\";

        public MainWindow()
        {
            InitializeComponent();
            IHighlightingDefinition definition = HighlightingLoader.Load(XmlReader.Create("Expression2.xshd"),
                                                             HighlightingManager.Instance);
            HighlightingManager.Instance.RegisterHighlighting("Expression2", new[] { ".txt" }, definition);
            foreach (string file in Directory.GetFiles(E2Path))
            {
                _fileList.Items.Add(Path.GetFileName(file));
            }
        }

        private void FileListMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _docPane.Items.Add(new E2EditorDocument((string)_fileList.SelectedItem));
        }
    }
}