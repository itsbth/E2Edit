using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using AvalonDock;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;

namespace E2Edit
{
    internal sealed class E2EditorDocument : DocumentContent
    {
        public E2EditorDocument(string fname)
        {
            var textEditor = new TextEditor
                                 {
                                     Text = File.ReadAllText(MainWindow.E2Path + fname),
                                     SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Expression2"),
                                     FontFamily = new FontFamily("Consolas")
                                 };
            textEditor.TextArea.TextView.LineTransformers.Add(new E2Colorizer());
            Title = fname;
            AddChild(textEditor);
            using (Stream s = new FileStream("funcs.txt", FileMode.Open))
            {
                IEnumerable<E2FunctionData> data = E2FunctionData.LoadData(s);
            }
        }
    }
}