using System.Collections.Generic;
using System.Linq;
using SCFirstOrderLogic.SentenceCreation;

namespace SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter8.UsingSentenceParser;

/// <summary>
/// <para>
/// The natural numbers example domain from chapter 8 of Artificial Intelligence: A Modern Approach, Global Edition by Stuart Russel and Peter Norvig.
/// </para>
/// <para>
/// Example usage:
/// </para>
/// <code>
/// IKnowledgeBase kb = .. // a knowledge base implementation
/// kb.Tell(NaturalNumbersDomain.Axioms);
/// kb.Tell(..facts about the specific problem..);
/// var answer = kb.Ask(..my query..);
/// </code>
/// </summary>
public static class NaturalNumbersDomain
{
    public static IReadOnlyCollection<Sentence> Axioms { get; } = new[]
    {
        "∀ x, ¬(Successor(x) = 0)",
        "∀ x, y, ¬[x = y] => ¬[Successor(x) = Successor(y)]",
        "∀ x, Add(0, x) = x",
        "∀ x, y, Add(Successor(x), y) = Add(Successor(y), x)",

    }.Select(s => SentenceParser.BasicParser.Parse(s)).ToList().AsReadOnly();
}
