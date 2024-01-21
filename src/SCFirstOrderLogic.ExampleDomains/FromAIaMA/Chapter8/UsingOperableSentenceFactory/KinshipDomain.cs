using System.Collections.Generic;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

namespace SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter8.UsingOperableSentenceFactory;

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
    public static IReadOnlyCollection<Sentence> Axioms { get; } = new List<Sentence>()
    {
        // One's mother is one's female parent:
        ForAll(M, C, Iff(AreEqual(Mother(C), M), IsFemale(M) & IsParent(M, C))),

        // Ones' husband is one's male spouse:
        ForAll(W, H, Iff(IsHusband(H, W), IsMale(H) & IsSpouse(H, W))),

        // Male and female are disjoint categories:
        ForAll(X, Iff(IsMale(X), !IsFemale(X))),

        // Parent and child are inverse relations:
        ForAll(P, C, Iff(IsParent(P, C), IsChild(C, P))),

        // A grandparent is a parent of one's parent:
        ForAll(G, C, Iff(IsGrandparent(G, C), ThereExists(P, IsParent(G, P) & IsParent(P, C)))),

        // A sibling is another child of one's parents:
        ForAll(X, Y, Iff(IsSibling(X, Y), !AreEqual(X, Y) & ThereExists(P, IsParent(P, X) & IsParent(P, Y)))),

    }.AsReadOnly();

    /// <summary>
    /// Gets some useful theorems of the kinship domain.
    /// Theorems are derivable from axioms, but might be useful for performance.
    /// </summary>
    public static IReadOnlyCollection<Sentence> Theorems { get; } = new List<Sentence>()
    {
        // Siblinghood is commutative:
        ForAll(X, Y, Iff(IsSibling(X, Y), IsSibling(Y, X))),

    }.AsReadOnly();

    //// Unary predicates:
    public static OperablePredicate IsMale(OperableTerm subject) => new Predicate(nameof(IsMale), subject);
    public static OperablePredicate IsFemale(OperableTerm subject) => new Predicate(nameof(IsFemale), subject);

    //// Binary predicates:
    public static OperablePredicate IsParent(OperableTerm subject, OperableTerm @object) => new Predicate(nameof(IsParent), subject, @object);
    public static OperablePredicate IsSibling(OperableTerm subject, OperableTerm @object) => new Predicate(nameof(IsSibling), subject, @object);
    public static OperablePredicate IsBrother(OperableTerm subject, OperableTerm @object) => new Predicate(nameof(IsBrother), subject, @object);
    public static OperablePredicate IsSister(OperableTerm subject, OperableTerm @object) => new Predicate(nameof(IsSister), subject, @object);
    public static OperablePredicate IsChild(OperableTerm subject, OperableTerm @object) => new Predicate(nameof(IsChild), subject, @object);
    public static OperablePredicate IsDaughter(OperableTerm subject, OperableTerm @object) => new Predicate(nameof(IsDaughter), subject, @object);
    public static OperablePredicate IsSon(OperableTerm subject, OperableTerm @object) => new Predicate(nameof(IsSon), subject, @object);
    public static OperablePredicate IsSpouse(OperableTerm subject, OperableTerm @object) => new Predicate(nameof(IsSpouse), subject, @object);
    public static OperablePredicate IsWife(OperableTerm subject, OperableTerm @object) => new Predicate(nameof(IsWife), subject, @object);
    public static OperablePredicate IsHusband(OperableTerm subject, OperableTerm @object) => new Predicate(nameof(IsHusband), subject, @object);
    public static OperablePredicate IsGrandparent(OperableTerm subject, OperableTerm @object) => new Predicate(nameof(IsGrandparent), subject, @object);
    public static OperablePredicate IsGrandchild(OperableTerm subject, OperableTerm @object) => new Predicate(nameof(IsGrandchild), subject, @object);
    public static OperablePredicate IsCousin(OperableTerm subject, OperableTerm @object) => new Predicate(nameof(IsCousin), subject, @object);
    public static OperablePredicate IsAunt(OperableTerm subject, OperableTerm @object) => new Predicate(nameof(IsAunt), subject, @object);
    public static OperablePredicate IsUncle(OperableTerm subject, OperableTerm @object) => new Predicate(nameof(IsUncle), subject, @object);

    //// Unary functions:
    public static OperableFunction Mother(OperableTerm t) => new Function(nameof(Mother), t);
    public static OperableFunction Father(OperableTerm t) => new Function(nameof(Father), t);
}
