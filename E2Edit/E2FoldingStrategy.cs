using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace E2Edit
{
    class E2FoldingStrategy : AbstractFoldingStrategy
    {
        public override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            throw new NotImplementedException();
        }
    }
}
