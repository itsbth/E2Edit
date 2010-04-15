using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
                                  Background = new SolidColorBrush(Color.FromRgb(0x1A, 0x1A, 0x1A)),
                                  Foreground = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                                  ShowLineNumbers = true,
                              };
            try
            {
                IEnumerable<Function> data;
                using (Stream s = new FileStream("Functions.xml", FileMode.Open))
                {
                    data = Function.LoadData(s);
                }
                _textEditor.TextArea.TextView.LineTransformers.Add(new E2Colorizer(data));
            }
            catch (Exception)
            {
                if (!DesignerProperties.GetIsInDesignMode(this)) MessageBox.Show("Unable to load function data.");
                Debugger.Break();
            }
            AddChild(_textEditor);
        }

        public string Text
        {
            get { return _textEditor.Text; }
        }

        public void Open(string fname)
        {
            _textEditor.Text = File.ReadAllText(fname);
        }

        public void Save(string fname)
        {
            File.WriteAllText(fname, _textEditor.Text);
        }
    }
}