using SCFirstOrderLogic.SentenceCreation;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingSentenceParser;

/// <summary>
/// <para>
/// The "crime" example from section 9.3 of Artificial Intelligence: A Modern Approach, Global Edition by Stuart Russel and Peter Norvig.
/// </para>
/// <para>
/// Example usage:
/// </para>
/// <code>
/// using SCFirstOrderLogic.SentenceCreation;
/// using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;
/// ..
/// IKnowledgeBase kb = .. // a knowledge base implementation
/// kb.Tell(Axioms);
/// var answer = kb.Ask(SentenceParser.Parse("IsCriminal(West)")); // should return true
/// </code>
/// </summary>
public static class CrimeDomain
{
    /// <summary>
    /// Gets the raw, unparsed version of <see cref="Axioms"/>.
    /// </summary>
    public static IReadOnlyCollection<string> UnparsedAxioms { get; } = new[]
    {
        // "... it is a crime for an American to sell weapons to hostile nations":
        "∀ x, y, z, IsAmerican(x) ∧ IsWeapon(y) ∧ Sells(x, y, z) ∧ IsHostile(z) ⇒ IsCriminal(x)",

        // "NoNo... has some missiles."
        "∃ x, IsMissile(x) ∧ Owns(NoNo, x)",

        // "All of its missiles were sold to it by Colonel West":
        "∀ x, IsMissile(x) ∧ Owns(NoNo, x) ⇒ Sells(ColonelWest, x, NoNo)",

        // We will also need to know that missiles are weapons: 
        "∀ x, IsMissile(x) ⇒ IsWeapon(x)",

        // And we must know that an enemy of America counts as "hostile":
        "∀ x, IsEnemyOf(x, America) ⇒ IsHostile(x)",

        // "West, who is American..":
        "IsAmerican(ColonelWest)",

        // "The country NoNo, an enemy of America..":
        "IsEnemyOf(NoNo, America)",
    };

    /// <summary>
    /// Gets the axioms of the crime domain.
    /// (okay, "IsAmerican(West)" isn't particularly fundamental, but..)
    /// </summary>
    public static IReadOnlyCollection<Sentence> Axioms => UnparsedAxioms.Select(s => SentenceParser.BasicParser.Parse(s)).ToList().AsReadOnly();

    public static string UnparsedExampleQuery { get; } = "IsCriminal(ColonelWest)";

    public static Sentence ExampleQuery => SentenceParser.BasicParser.Parse(UnparsedExampleQuery);
}
