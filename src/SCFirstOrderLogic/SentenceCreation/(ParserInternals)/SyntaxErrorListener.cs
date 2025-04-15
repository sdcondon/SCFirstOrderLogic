using Antlr4.Runtime;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace SCFirstOrderLogic.SentenceCreation;

internal class SyntaxErrorListener : BaseErrorListener
{
    private readonly List<SyntaxError> errors = new();

    public SyntaxErrorListener()
    {
        Errors = new ReadOnlyCollection<SyntaxError>(errors);
    }

    public IReadOnlyCollection<SyntaxError> Errors { get; }

    public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        errors.Add(new SyntaxError(line, charPositionInLine, offendingSymbol.Text, msg, e));
    }
}
