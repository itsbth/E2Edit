using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using AvalonDock;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;

namespace E2Edit
{
    internal sealed class E2EditorDocument : UserControl
    {
        private readonly TextEditor _textEditor;

        public E2EditorDocument()
        {
            _textEditor = new TextEditor
                              {
                                  SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Expression2"),
                                  FontFamily = new FontFamily("Consolas"),
                                  Background = new SolidColorBrush(Color.FromRgb(0x32, 0x32, 0x32)),
                                  Foreground = Brushes.White,
                                  ShowLineNumbers = true,
                              };
            try
            {
                IEnumerable<E2FunctionData> data;
                using (Stream s = new FileStream("funcs.txt", FileMode.Open))
                {
                    data = E2FunctionData.LoadData(s);
                }
                _textEditor.TextArea.TextView.LineTransformers.Add(new E2Colorizer(data));
            }
            catch (System.Exception)
            {
                // Do nothing
            }
            AddChild(_textEditor);
        }

        public void Open(string fname)
        {
            _textEditor.Text = File.ReadAllText(fname);
        }
    }
}