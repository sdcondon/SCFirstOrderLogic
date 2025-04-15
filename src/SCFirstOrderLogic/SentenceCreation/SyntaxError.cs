using System;

namespace SCFirstOrderLogic.SentenceCreation;

/// <summary>
/// Container for information about a syntax error encountered while attempting to parse 
/// </summary>
/// <param name="LineNumber">The (1-based) line on which the error starts.</param>
/// <param name="CharacterIndex">The (0-based) character index at which the error starts.</param>
/// <param name="OffendingText">The offending text.</param>
/// <param name="Message">The details of the error.</param>
/// <param name="Exception">The underlying exception, raised by the low-level parser logic.</param>
public record SyntaxError(int LineNumber, int CharacterIndex, string OffendingText, string Message, Exception Exception)
{
    /// <inheritdoc />
    public override string ToString()
    {
        return $"line {LineNumber}, char {CharacterIndex}, offending text '{OffendingText}': {Message}";
    }
}
