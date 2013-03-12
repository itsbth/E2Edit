using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using E2Edit.Resources;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;

namespace E2Edit.Editor
{
    internal sealed class E2Editor : UserControl
    {
        private readonly IList<Function> _functionData;
        private readonly Settings _settings;
        private readonly TextEditor _textEditor;
        private CompletionWindow _completionWindow;

        public E2Editor()
        {
            _settings = MainWindow.Settings ?? new Settings();
            _textEditor = new TextEditor
                              {
                                  SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Expression2"),
                                  Background = new SolidColorBrush(Color.FromRgb(0x1A, 0x1A, 0x1A)),
                                  Foreground = new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0)),
                                  FontFamily = _settings.Font,
                                  ShowLineNumbers = true,
                                  Options = {ConvertTabsToSpaces = true},
                              };
            if (_settings.AutoIndentEnabled)
            {
                _textEditor.TextArea.IndentationStrategy = new E2IndentationStrategy(_textEditor.Options);
                _textEditor.TextArea.TextEntered += AutoIndent_OnTextEntered;
            }
            try
            {
                using (Stream s = Resource.GetResource("E2.Functions"))
                    _functionData = Function.LoadData(s);
                _textEditor.TextArea.TextView.LineTransformers.Add(new E2Colorizer(_functionData));
                _textEditor.TextArea.TextEntered += IntelliSense_OnTextEntered;
                _textEditor.TextArea.TextEntered += Insight_OnTextEntered;
            }
            catch (Exception)
            {
                if (!DesignerProperties.GetIsInDesignMode(this)) MessageBox.Show("Unable to load function data.");
                //Debugger.Break();
            }
            AddChild(_textEditor);
        }

        public string Text
        {
            get { return _textEditor.Text; }
            set { _textEditor.Text = value; }
        }

        public bool IsModified
        {
            get { return _textEditor.CanUndo; }
        }

        private void AutoIndent_OnTextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "}" || !_settings.AutoIndentEnabled) return;
            TextDocument document = _textEditor.Document;
            DocumentLine line = document.GetLineByOffset(_textEditor.TextArea.Caret.Offset);
            string text = document.GetText(line);
            if (text.StartsWith(_textEditor.Options.IndentationString) && text.EndsWith("}") && !text.Contains("{"))
                document.Remove(line.Offset, _textEditor.Options.IndentationSize);
        }

        private void IntelliSense_OnTextEntered(object sender, TextCompositionEventArgs e)
        {
            if (_completionWindow != null && !Char.IsLetterOrDigit(e.Text[0]))
            {
                _completionWindow.Close();
                _completionWindow = null;
            }
            if (e.Text != ":" || _completionWindow != null || !_settings.IntelliSenseEnabled ||
                _textEditor.Document.GetText(_textEditor.Document.GetLineByOffset(_textEditor.CaretOffset)).StartsWith(
                    "@"))
                return;
            _completionWindow = new CompletionWindow(_textEditor.TextArea);
            foreach (Function function in _functionData.Where(f => f.ThisType != DataType.Void).OrderBy((f => f.Name)))
            {
                _completionWindow.CompletionList.CompletionData.Add(function);
            }
            _completionWindow.Closed += (s, ex) => _completionWindow = null;
            _completionWindow.Show();
        }

        private void Insight_OnTextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "(") return;
            var t = this;
        }

        public void Open(string fname)
        {
            _textEditor.Document = new TextDocument(File.ReadAllText(fname));
        }

        public void Save(string fname)
        {
            File.WriteAllText(fname, _textEditor.Text);
            _textEditor.Document.UndoStack.ClearAll();
        }
    }
}