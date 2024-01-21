using SCFirstOrderLogic.LanguageIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static SCFirstOrderLogic.LanguageIntegration.Operators;

namespace SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter8.UsingLanguageIntegration;

/// <summary>
/// <para>
/// The kinship example domain from chapter 8 of Artificial Intelligence: A Modern Approach, Global Edition by Stuart Russel and Peter Norvig.
/// </para>
/// <para>
/// Example usage:
/// </para>
/// <code>
/// ILinqKnowledgeBase&lt;IPerson&gt; kb = .. // a LINQ knowledge base implementation
/// kb.Tell(KinshipDomain.Axioms);
/// kb.Tell(..facts about the specific problem..);
/// // ..though its worth noting that language integration allows for KBs that include stuff like
/// // BindConstants(domainAdapter, options), where domainAdapter is an IEnumerable&lt;IPerson&gt;,
/// // to specify known constants (i.e. their relationships/functions) in an OO way that easily integrates
/// // with the rest of the app. Note how you'd *need* some kind of "options" to set how unspecified
/// // predicates and functions involving constants are expressed. Thrown exceptions? Null return values
/// // (for functions)? etc. And there are some subtleties and complications here (particularly once you 
/// // start thinking about non-unary predicates and functions) that don't necessarily have any, let alone
/// // easy, answers. So, no such KBs written just yet.
/// var answer = kb.Ask(..my query..);
/// </code>
/// </summary>
public static class KinshipDomain
{
    /// <summary>
    /// Gets the fundamental axioms of the kinship domain.
    /// </summary>
    public static IReadOnlyCollection<Expression<Predicate<IEnumerable<IPerson>>>> Axioms { get; } = new List<Expression<Predicate<IEnumerable<IPerson>>>>()
    {
        // One's mother is one's female parent:
        d => d.All((m, c) => Iff(c.Mother == m, m.IsFemale && m.IsParentOf(c))),

        // Ones' husband is one's male spouse:
        d => d.All((w, h) => Iff(h.IsHusbandOf(w), h.IsMale && h.IsSpouseOf(w))),

        // Male and female are disjoint categories:
        d => d.All(x => Iff(x.IsMale, !x.IsFemale)),

        // Parent and child are inverse relations:
        d => d.All((p, c) => Iff(p.IsParentOf(c), c.IsChildOf(p))),

        // A grandparent is a parent of one's parent:
        d => d.All((g, c) => Iff(g.IsGrandparentOf(c), d.Any(p => g.IsParentOf(p) && p.IsParentOf(c)))),

        // A sibling is another child of one's parents:
        d => d.All((x, y) => Iff(x.IsSiblingOf(y), x != y && d.Any(p => p.IsParentOf(x) && p.IsParentOf(y)))),

    }.AsReadOnly();

    /// <summary>
    /// Gets some useful theorems of the kinship domain.
    /// Theorems are derivable from axioms, but might be useful for performance.
    /// </summary>
    public static IReadOnlyCollection<Expression<Predicate<IEnumerable<IPerson>>>> Theorems { get; } = new List<Expression<Predicate<IEnumerable<IPerson>>>>()
    {
        // Siblinghood is commutative:
        d => d.All((x, y) => Iff(x.IsSiblingOf(y), y.IsSiblingOf(x))),

    }.AsReadOnly();
}

/// <summary>
/// Interface for the elements of the <see cref="KinshipDomain"/>. An implementation would only be needed for the <c>Bind</c> scenario mentioned in summary of the <see cref="KinshipDomain"/> class.
/// </summary>
public interface IPerson
{
    //// Unary predicates:
    bool IsMale { get; }
    bool IsFemale { get; }

    //// Binary predicates:
    bool IsParentOf(IPerson person);
    bool IsSiblingOf(IPerson person);
    bool IsBrotherOf(IPerson person);
    bool IsSisterOf(IPerson person);
    bool IsChildOf(IPerson person);
    bool IsDaughterOf(IPerson person);
    bool IsSonOf(IPerson person);
    bool IsSpouseOf(IPerson person);
    bool IsWifeOf(IPerson person);
    bool IsHusbandOf(IPerson person);
    bool IsGrandparentOf(IPerson person);
    bool IsGrandchildOf(IPerson person);
    bool IsCousinOf(IPerson person);
    bool IsAuntOf(IPerson person);
    bool IsUncleOf(IPerson person);

    //// Unary functions:
    IPerson Mother { get; }
    IPerson Father { get; }
}
