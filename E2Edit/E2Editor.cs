using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;

namespace E2Edit
{
    internal sealed class E2Editor : UserControl
    {
        private readonly TextEditor _textEditor;
        private readonly IList<Function> _functionData;
        private CompletionWindow _completionWindow;

        public E2Editor()
        {
            _textEditor = new TextEditor
                              {
                                  SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Expression2"),
                                  FontFamily = new FontFamily("Consolas"),
                                  Background = new SolidColorBrush(Color.FromRgb(0x1A, 0x1A, 0x1A)),
                                  Foreground = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                                  ShowLineNumbers = true,
                                  Options = {ConvertTabsToSpaces = true},
                              };
            _textEditor.TextArea.IndentationStrategy = new E2IndentationStrategy(_textEditor.Options);
            try
            {
                using (Stream s = new FileStream("Functions.xml", FileMode.Open))
                    _functionData = Function.LoadData(s);
                _textEditor.TextArea.TextView.LineTransformers.Add(new E2Colorizer(_functionData));
                _textEditor.TextArea.TextEntered += IntelliSense_OnTextEntered;
            }
            catch (Exception)
            {
                if (!DesignerProperties.GetIsInDesignMode(this)) MessageBox.Show("Unable to load function data.");
                Debugger.Break();
            }
            AddChild(_textEditor);
        }

        private void IntelliSense_OnTextEntered(object sender, TextCompositionEventArgs e)
        {
            if (_completionWindow != null && !Char.IsLetterOrDigit(e.Text[0]))
            {
                _completionWindow.Close();
                _completionWindow = null;
                return;
            }
            if (e.Text != ":" || _completionWindow != null)
                return;
            _completionWindow = new CompletionWindow(_textEditor.TextArea);
            foreach (var function in _functionData.OrderBy((f => f.Name)))
            {
                _completionWindow.CompletionList.CompletionData.Add(function);
            }
            _completionWindow.Closed += (s, ex) => _completionWindow = null;
            _completionWindow.Show();
        }

        public string Text
        {
            get { return _textEditor.Text; }
            set { _textEditor.Text = value; }
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