using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace E2Edit.Editor
{
    internal class E2Colorizer : DocumentColorizingTransformer
    {
        private readonly IEnumerable<Function> _data;

        public E2Colorizer(IEnumerable<Function> data)
        {
            _data = data;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            int lineStartOffset = line.Offset;
            string text = CurrentContext.Document.GetText(line);
            if (text.StartsWith("@"))
            {
                // TODO: Color directives
            }
            else
            {
                ColorFunctions(text, lineStartOffset);
            }
        }

        private void ColorFunctions(string text, int lineStartOffset)
        {
            int start = -1;
            var buff = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (!char.IsLetter(text[i]))
                {
                    if (buff.Length > 0)
                    {
                        string str = buff.ToString();
                        ColorFunction(str, lineStartOffset + start);
                        buff.Remove(0, buff.Length);
                        start = -1;
                    }
                }
                else
                {
                    if (start == -1) start = i;
                    buff.Append(text[i]);
                }
            }
            if (buff.Length != 0)
            {
                ColorFunction(buff.ToString(), lineStartOffset + start);
            }
        }

        private void ColorFunction(string str, int start)
        {
            SolidColorBrush brush = _data.Any(func => func.Name == str)
                                        ? new SolidColorBrush(Color.FromRgb(160, 160, 240))
                                        : Brushes.Red;

            ChangeLinePart(
                start, // startOffset
                start + str.Length, // endOffset
                element =>
                    {
                        if ((element.TextRunProperties.ForegroundBrush is SolidColorBrush &&
                             (element.TextRunProperties.ForegroundBrush as SolidColorBrush).Color ==
                             Color.FromRgb(224, 224, 224)))
                            element.TextRunProperties.SetForegroundBrush(
                                brush);
                    });
        }
    }
}