using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static SCFirstOrderLogic.LanguageIntegration.Operators;

namespace SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter8.Linq
{
    /// <summary>
    /// The natural numbers example domain from chapter 8 of Artificial Intelligence: A Modern Approach, Global Edition by Stuart Russel and Peter Norvig.
    /// <para/>
    /// Example usage:
    /// <code>
    /// ILinqKnowledgeBase&lt;INaturalNumbers, INaturalNumber&gt; kb = .. // a LINQ knowledge base implementation
    /// kb.Tell(NaturalNumbersDomain.Axioms);
    /// kb.Tell(..facts about the specific problem..);
    /// // .. though the real value of language integration would be in KBs that support something like kb.Bind(domainAdapter, opts),
    /// // where domainAdapter is an INaturalNumbers, to specify known constants in a way that easily integrates with the rest of the app
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
}
