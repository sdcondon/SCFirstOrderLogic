using SCFirstOrderLogic.LanguageIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static SCFirstOrderLogic.LanguageIntegration.Operators;

namespace SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter8.UsingLanguageIntegration;

/// <summary>
/// <para>
/// The natural numbers example domain from chapter 8 of Artificial Intelligence: A Modern Approach, Global Edition by Stuart Russel and Peter Norvig.
/// </para>
/// <para>
/// Example usage:
/// </para>
/// <code>
/// ILinqKnowledgeBase&lt;INaturalNumbers, INaturalNumber&gt; kb = .. // a LINQ knowledge base implementation
/// kb.Tell(NaturalNumbersDomain.Axioms);
/// kb.Tell(..facts about the specific problem..);
/// // ..though its worth noting that language integration allows for KBs that include stuff like
/// // BindConstants(domainAdapter, options), where domainAdapter is an INaturalNumbers,
/// // to specify known constants (i.e. their relationships/functions) in an OO way that easily integrates
/// // with the rest of the app. Note how you'd *need* some kind of "options" to set how unspecified
/// // predicates and functions involving constants are expressed. Thrown exceptions? Null return values
/// // (for functions)? etc. And there are some subtleties and complications here (particularly once you 
/// // start thinking about non-unary predicates and functions) that don't necessarily have any, let alone
/// // easy, answers. So, no such KBs written just yet.
/// var answer = kb.Ask(..my query..);
/// </code>
/// </summary>
public static class NaturalNumbersDomain
{
    /// <summary>
    /// Gets the fundamental axioms of the natural numbers domain.
    /// </summary>
    public static IReadOnlyCollection<Expression<Predicate<INaturalNumbers>>> Axioms { get; } = new List<Expression<Predicate<INaturalNumbers>>>()
    {
        d => d.All(x => x.Successor != d.Zero),
        d => d.All((x, y) => If(x != y, x.Successor != y.Successor)),
        d => d.All(x => d.Zero.Add(x) == x),
        d => d.All((x, y) => x.Successor.Add(y) == x.Add(y).Successor),

    }.AsReadOnly();
}

/// <summary>
/// The domain type for the <see cref="NaturalNumbersDomain"/>.
/// </summary>
public interface INaturalNumbers : IEnumerable<INaturalNumber>
{
    INaturalNumber Zero { get; }
}

/// <summary>
/// Interface for the elements of the <see cref="NaturalNumbersDomain"/>. An implementation would only be needed for the <c>Bind</c> scenario mentioned in summary of the <see cref="NaturalNumbersDomain"/> class.
/// </summary>
public interface INaturalNumber
{
    INaturalNumber Successor { get; }

    INaturalNumber Add(INaturalNumber x);
}
