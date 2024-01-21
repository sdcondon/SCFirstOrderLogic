using System.Collections.Generic;
using System.Linq;
using SCFirstOrderLogic.SentenceCreation;

namespace SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter8.UsingSentenceParser;

/// <summary>
/// The kinship example domain from chapter 8 of Artificial Intelligence: A Modern Approach, Global Edition by Stuart Russel and Peter Norvig.
/// Example usage:
/// <code>
/// IKnowledgeBase kb = .. // a knowledge base implementation
/// kb.Tell(KinshipDomain.Axioms);
/// kb.Tell(..facts about the specific problem..);
/// var answer = kb.Ask(..my query..);
/// </code>
/// </summary>
public static class KinshipDomain
{
    /// <summary>
    /// Gets the fundamental axioms of the kinship domain.
    /// </summary>
    public static IReadOnlyCollection<Sentence> Axioms { get; } = new[]
    {
        // One's mother is one's female parent:
        "∀ m, c, [Mother(c) = m] ⇔ [IsFemale(m) ∧ IsParent(m, c)]",

        // Ones' husband is one's male spouse:
        "∀ w, h, IsHusband(h, w) ⇔ [IsMale(h) ∧ IsSpouse(h, w)]",

        // Male and female are disjoint categories:
        "∀ x, IsMale(x) ⇔ ¬IsFemale(x)",

        // Parent and child are inverse relations:
        "∀ p, c, IsParent(p, c) ⇔ IsChild(c, p)",

        // A grandparent is a parent of one's parent:
        "∀ g, c, IsGrandParent(g, c) ⇔ [∃ p, IsParent(g, p) ∧ IsParent(p, c)]",

        // A sibling is another child of one's parents:
        "∀ x, y, IsSibling(x, y) ⇔ [¬(x = y) ∧ [∃ p, IsParent(p, x) ∧ IsParent(p, y)]]",

    }.Select(s => SentenceParser.BasicParser.Parse(s)).ToList().AsReadOnly();

    /// <summary>
    /// Gets some useful theorems of the kinship domain.
    /// Theorems are derivable from axioms, but might be useful for performance.
    /// </summary>
    public static IReadOnlyCollection<Sentence> Theorems { get; } = new[]
    {
        // Siblinghood is commutative:
        "∀ x, y, IsSibling(x, y) ⇔ IsSibling(y, x)",

    }.Select(s => SentenceParser.BasicParser.Parse(s)).ToList().AsReadOnly();
}
