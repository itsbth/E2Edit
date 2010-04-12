using System.Text;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace E2Edit
{
    public class E2Colorizer : DocumentColorizingTransformer
    {
        protected override void ColorizeLine(DocumentLine line)
        {
            int lineStartOffset = line.Offset;
            string text = CurrentContext.Document.GetText(line);
            int start = 0;
            int index;
            var buff = new StringBuilder();
            while ((index = text.IndexOf("AvalonEdit", start)) >= 0)
            {
                ChangeLinePart(
                    lineStartOffset + index, // startOffset
                    lineStartOffset + index + 10, // endOffset
                    element => element.TextRunProperties.SetForegroundBrush(Brushes.Green));
                start = index + 1; // search for next occurrence
            }
        }
    }
}