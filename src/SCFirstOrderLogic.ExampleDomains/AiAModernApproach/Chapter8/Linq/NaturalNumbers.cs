using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static SCFirstOrderLogic.LanguageIntegration.Operators;

namespace SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter8.NaturalNumbers.Linq
{
    public interface INaturalNumbers : IEnumerable<INaturalNumber>
    {
        INaturalNumber Zero { get; }
    }

    public interface INaturalNumber
    {
        INaturalNumber Successor { get; }

        INaturalNumber Add(INaturalNumber x);
    }

    /// <summary>
    /// Container for fundamental knowledge about the <see cref="INaturalNumbers"/> domain.
    /// </summary>
    public static class NaturalNumberKnowledge
    {
        // Usage (note the separation of concerns for knowledge base implementation and the domain):
        //
        // var kb = new ResolutionKnowledgeBase<INaturalNumbers, INaturalNumber>(); // ..or a different KB implementation - none implemented yet
        // kb.Tell(NaturalNumberKnowledge.Axioms);
        // kb.Tell(..facts about the specific problem..);
        // .. though the real value of language integration would be in allowing something like kb.Bind(domainAdapter, opts),
        //    where domainAdapter is an INaturalNumbers, to specify known constants in a way that easily integrates with.. other things
        // kb.Ask(..my query..);
        public static IReadOnlyCollection<Expression<Predicate<INaturalNumbers>>> Axioms { get; } = new List<Expression<Predicate<INaturalNumbers>>>()
        {
            d => d.All(x => x.Successor != d.Zero),
            d => d.All((x, y) => If(x != y, x.Successor != y.Successor)),
            d => d.All(x => d.Zero.Add(x) == x),
            d => d.All((x, y) => x.Successor.Add(y) == x.Add(y).Successor),

        }.AsReadOnly();
    }
}
