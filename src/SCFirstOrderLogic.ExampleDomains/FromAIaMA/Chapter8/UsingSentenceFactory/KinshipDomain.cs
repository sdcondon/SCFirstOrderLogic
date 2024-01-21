using System.Collections.Generic;
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;

namespace SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter8.UsingSentenceFactory;

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
        ForAll(M, C, Iff(AreEqual(Mother(C), M), And(IsFemale(M), IsParent(M, C)))),

        // Ones' husband is one's male spouse:
        ForAll(W, H, Iff(IsHusband(H, W), And(IsMale(H), IsSpouse(H, W)))),

        // Male and female are disjoint categories:
        ForAll(X, Iff(IsMale(X), Not(IsFemale(X)))),

        // Parent and child are inverse relations:
        ForAll(P, C, Iff(IsParent(P, C), IsChild(C, P))),

        // A grandparent is a parent of one's parent:
        ForAll(G, C, Iff(IsGrandparent(G, C), ThereExists(P, And(IsParent(G, P), IsParent(P, C))))),

        // A sibling is another child of one's parents:
        ForAll(X, Y, Iff(IsSibling(X, Y), And(Not(AreEqual(X, Y)), ThereExists(P, And(IsParent(P, X), IsParent(P, Y)))))),

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
    public static Predicate IsMale(Term subject) => new(nameof(IsMale), subject);
    public static Predicate IsFemale(Term subject) => new(nameof(IsFemale), subject);

    //// Binary predicates:
    public static Predicate IsParent(Term subject, Term @object) => new(nameof(IsParent), subject, @object);
    public static Predicate IsSibling(Term subject, Term @object) => new(nameof(IsSibling), subject, @object);
    public static Predicate IsBrother(Term subject, Term @object) => new(nameof(IsBrother), subject, @object);
    public static Predicate IsSister(Term subject, Term @object) => new(nameof(IsSister), subject, @object);
    public static Predicate IsChild(Term subject, Term @object) => new(nameof(IsChild), subject, @object);
    public static Predicate IsDaughter(Term subject, Term @object) => new(nameof(IsDaughter), subject, @object);
    public static Predicate IsSon(Term subject, Term @object) => new(nameof(IsSon), subject, @object);
    public static Predicate IsSpouse(Term subject, Term @object) => new(nameof(IsSpouse), subject, @object);
    public static Predicate IsWife(Term subject, Term @object) => new(nameof(IsWife), subject, @object);
    public static Predicate IsHusband(Term subject, Term @object) => new(nameof(IsHusband), subject, @object);
    public static Predicate IsGrandparent(Term subject, Term @object) => new(nameof(IsGrandparent), subject, @object);
    public static Predicate IsGrandchild(Term subject, Term @object) => new(nameof(IsGrandchild), subject, @object);
    public static Predicate IsCousin(Term subject, Term @object) => new(nameof(IsCousin), subject, @object);
    public static Predicate IsAunt(Term subject, Term @object) => new(nameof(IsAunt), subject, @object);
    public static Predicate IsUncle(Term subject, Term @object) => new(nameof(IsUncle), subject, @object);

    //// Unary functions:
    public static Function Mother(Term t) => new(nameof(Mother), t);
    public static Function Father(Term t) => new(nameof(Father), t);
}
