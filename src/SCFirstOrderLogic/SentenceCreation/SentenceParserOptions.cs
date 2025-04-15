using System;

namespace SCFirstOrderLogic.SentenceCreation;

/// <summary>
/// Container for configuration options for <see cref="SentenceParser"/> instances.
/// </summary>
/// <param name="GetPredicateIdentifier">A delegate to retrieve the identifier for a predicate, given its symbol text.</param>
/// <param name="GetFunctionIdentifier">A delegate to retrieve the identifier for a function, given its symbol text.</param>
/// <param name="GetVariableOrConstantIdentifier">
/// <para>
/// A delegate to retrieve the identifier for a variable reference or a constant, given its symbol text.
/// </para>
/// <para>
/// These are combined because we check whether the returned value is among the identifiers of variables
/// that are in scope in order to determine whether something is a variable reference or a zero arity function
/// without parentheses. If they were separate, you'd end up in the awkward situation where you'd have to bear
/// in mind that "getVariableIdentifier" actually gets called for things that are zero arity functions, and for
/// zero arity functions two "gets" would end up being performed. Or of course we could offer the ability to 
/// customise this determination logic - which feels overcomplicated.
/// </para>
/// </param>
public record SentenceParserOptions(
    Func<string, object> GetPredicateIdentifier,
    Func<string, object> GetFunctionIdentifier,
    Func<string, object> GetVariableOrConstantIdentifier)
{
    /// <summary>
    /// Default options, that just use the symbol text as the identifier for returned predicates, functions and variables.
    /// </summary>
    public static SentenceParserOptions Default { get; } = new(s => s, s => s, s => s);
}
