using Antlr4.Runtime;
using SCFirstOrderLogic.SentenceCreation.Antlr;

namespace SCFirstOrderLogic.SentenceCreation;

internal static class AntlrParserFactory
{
    public static FirstOrderLogicParser MakeParser(AntlrInputStream inputStream)
    {
        // NB: ANTLR apparently adds a listener by default that writes to the console.
        // Which is crazy default behaviour if you ask me, but never mind.
        // We remove it so that consumers of this lib don't get random messages turning up on their console.
        FirstOrderLogicLexer lexer = new(inputStream);
        lexer.RemoveErrorListeners();
        CommonTokenStream tokens = new(lexer);

        // NB: In the parser, we add our own error listener that throws an exception.
        // Otherwise errors would just be ignored and the method would just return null, which is obviously bad behaviour.
        FirstOrderLogicParser parser = new(tokens);
        parser.RemoveErrorListeners();
        parser.AddErrorListener(ThrowingErrorListener.Instance);

        return parser;
    }
}
