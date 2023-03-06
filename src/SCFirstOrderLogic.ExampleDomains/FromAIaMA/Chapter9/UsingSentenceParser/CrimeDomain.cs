using SCFirstOrderLogic.SentenceCreation;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingSentenceParser
{
    /// <summary>
    /// <para>
    /// The "crime" example from section 9.3 of Artificial Intelligence: A Modern Approach, Global Edition by Stuart Russel and Peter Norvig.
    /// </para>
    /// <para>
    /// Example usage:
    /// </para>
    /// <code>
    /// using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;
    /// ..
    /// IKnowledgeBase kb = .. // a knowledge base implementation
    /// kb.Tell(Axioms);
    /// var answer = kb.Ask(IsCriminal(West)); // should return true
    /// </code>
    /// </summary>
    public static class CrimeDomain
    {
        /// <summary>
        /// Gets the fundamental axioms of the crime domain.
        /// </summary>
        public static IReadOnlyCollection<Sentence> Axioms { get; } = new[]
        {
            // "... it is a crime for an American to sell weapons to hostile nations":
            "∀ x, y, z, IsAmerican(x) ∧ IsWeapon(y) ∧ Sells(x, y, z) ∧ IsHostile(z) ⇒ IsCriminal(x)",

            // "Nono... has some missiles."
            "∃ x, IsMissile(x) ∧ Owns(Nono, x)",

            // "All of its missiles were sold to it by Colonel West":
            "∀ x, IsMissile(x) ∧ Owns(Nono, x) ⇒ Sells(West, x, Nono)",

            // We will also need to know that missiles are weapons: 
            "∀ x, IsMissile(x) ⇒ IsWeapon(x)",

            // And we must know that an enemy of America counts as "hostile":
            "∀ x, IsEnemyOf(x, America) ⇒ IsHostile(x)",

            // "West, who is American..":
            "IsAmerican(West)",

            // "The country Nono, an enemy of America..":
            "IsEnemyOf(Nono, America)",

        }.Select(s => SentenceParser.Parse(s)).ToList().AsReadOnly();
    }
}
