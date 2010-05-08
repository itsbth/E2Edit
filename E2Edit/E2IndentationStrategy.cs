using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;

namespace E2Edit
{
    internal class E2IndentationStrategy : DefaultIndentationStrategy
    {
        private readonly TextEditorOptions _options;

        public E2IndentationStrategy(TextEditorOptions options)
        {
            _options = options;
        }

        public override void IndentLine(TextDocument document, DocumentLine line)
        {
            base.IndentLine(document, line);
            if (document.GetText(line.PreviousLine).Trim().EndsWith("{"))
                document.Insert(line.Offset, _options.IndentationString);
        }
    }
}