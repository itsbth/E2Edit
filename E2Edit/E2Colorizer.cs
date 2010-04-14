using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace E2Edit
{
    internal class E2Colorizer : DocumentColorizingTransformer
    {
        private readonly IEnumerable<E2FunctionData> _data;

        public E2Colorizer(IEnumerable<E2FunctionData> data)
        {
            _data = data;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            int lineStartOffset = line.Offset;
            string text = CurrentContext.Document.GetText(line);
            int start = -1;
            var buff = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (!char.IsLetter(text[i]))
                {
                    if (buff.Length > 0)
                    {
                        string str = buff.ToString();
                        if (_data.Any(func => func.Name == str))
                            ChangeLinePart(
                                lineStartOffset + start, // startOffset
                                lineStartOffset + start + buff.Length, // endOffset
                                element =>
                                    {
                                        if ((element.TextRunProperties.ForegroundBrush is SolidColorBrush &&
                                              (element.TextRunProperties.ForegroundBrush as SolidColorBrush).Color == Color.FromRgb(224, 224, 224)))
                                            element.TextRunProperties.SetForegroundBrush(
                                                new SolidColorBrush(Color.FromRgb(160, 160, 240)));
                                    });
                        buff.Clear();
                        start = -1;
                    }
                }
                else
                {
                    if (start == -1) start = i;
                    buff.Append(text[i]);
                }
            }
        }
    }
}