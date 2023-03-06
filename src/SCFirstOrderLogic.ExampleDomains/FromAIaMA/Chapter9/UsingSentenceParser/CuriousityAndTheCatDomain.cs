using System.Collections.Generic;
using System.Linq;
using SCFirstOrderLogic.SentenceCreation;

namespace SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingSentenceParser
{
    /// <summary>
    /// <para>
    /// The "curiousity and the cat" example from section 9.5 of Artificial Intelligence: A Modern Approach,
    /// Global Edition by Stuart Russel and Peter Norvig.
    /// </para>
    /// <para>
    /// Example usage:
    /// </para>
    /// <code>
    /// using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CuriousityAndTheCatDomain;
    /// ..
    /// IKnowledgeBase kb = .. // a knowledge base implementation
    /// kb.Tell(Axioms);
    /// var answer = kb.Ask(Kills(Curiousity, Tuna)); // should return true
    /// </code>
    /// </summary>
    public static class CuriousityAndTheCatDomain
    {
        /// <summary>
        /// Gets the fundamental axioms of the domain.
        /// </summary>
        public static IReadOnlyCollection<Sentence> Axioms { get; } = new[]
        {
            // Everyone who loves all animals is loved by someone.
            "∀x [∀y Animal(y) ⇒ Loves(x, y)] ⇒ [∃y Loves(y, x)]",

            // Anyone who kills an animal is loved by no one.
            "∀x [∃z Animal(z) ∧ Kills(x, z)] ⇒ [∀y ¬Loves(y, x)]",

            // Jack loves all animals.
            "∀x Animal(x) ⇒ Loves(Jack, x)",

            // Either Jack or Curiosity killed the cat, who is named Tuna.
            "Kills(Jack, Tuna) ∨ Kills(Curiousity, Tuna)",
            "Cat(Tuna)",

            // Cats are animals.
            "∀x Cat(x) ⇒ Animal(x)",

        }.Select(s => SentenceParser.Parse(s)).ToList().AsReadOnly();
    }
}
