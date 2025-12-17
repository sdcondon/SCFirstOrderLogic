using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SCFirstOrderLogic.FormulaCreation;

/// <summary>
/// Exception type used when syntax errors are encountered by <see cref="FormulaParser"/>.
/// </summary>
public class SyntaxErrorsException : Exception
{
    /// <summary>
    /// Initialises a new instance of the <see cref="SyntaxErrorsException"/> class.
    /// </summary>
    /// <param name="errors">The errors that were encountered.</param>
    public SyntaxErrorsException(IList<SyntaxError> errors)
        : base(MakeMessage(errors))
    {
        Errors = new ReadOnlyCollection<SyntaxError>(errors);
    }

    /// <summary>
    /// Gets the collection of syuntax errors that were encountered. 
    /// </summary>
    public ReadOnlyCollection<SyntaxError> Errors { get; }

    private static string MakeMessage(IList<SyntaxError> errors)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("One or more syntax errors were observed:");

        for (int i = 0; i < errors.Count; i++)
        {
            stringBuilder.AppendLine($"- Error #{i + 1}: {errors[i]}");
        }

        return stringBuilder.ToString();
    }
}
