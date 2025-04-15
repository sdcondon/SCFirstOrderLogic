using Antlr4.Runtime;
using System;
using System.IO;

namespace SCFirstOrderLogic.SentenceCreation;

internal class ThrowingErrorListener : BaseErrorListener
{
    public static ThrowingErrorListener Instance = new();

    public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        throw new ArgumentException("line " + line + ":" + charPositionInLine + " " + msg, "sentence");
    }
}
