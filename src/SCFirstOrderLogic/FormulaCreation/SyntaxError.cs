using System;

namespace SCFirstOrderLogic.FormulaCreation;

/// <summary>
/// Container for information about a syntax error encountered while attempting to parse.
/// </summary>
/// <param name="LineNumber">The (1-based) line on which the error starts.</param>
/// <param name="CharacterIndex">The (0-based) character index at which the error starts.</param>
/// <param name="OffendingTokenText">The offending token text, or null for a lexer error.</param>
/// <param name="Message">The details of the error.</param>
/// <param name="Exception">The underlying exception, raised by the low-level parser logic.</param>
public record SyntaxError(int LineNumber, int CharacterIndex, string? OffendingTokenText, string Message, Exception Exception)
{
    /// <inheritdoc />
    public override string ToString()
    {
        var badTokenDetails = OffendingTokenText != null ? $", offending token '{OffendingTokenText}'" : string.Empty;
        return $"line {LineNumber}, char {CharacterIndex}{badTokenDetails}: {Message}";
    }
}
